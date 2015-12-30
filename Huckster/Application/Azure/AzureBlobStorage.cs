using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.Shared.Protocol;

namespace Application.Azure
{
    public class BlobProperties
    {
        public static BlobProperties JpegContentTypeDefault
        {
            get { return new BlobProperties() { ContentTyp = "image/jpeg" }; }
        }

        public static BlobProperties PDF
        {
            get { return new BlobProperties() { ContentTyp = "application/pdf" }; }
        }

        public static BlobProperties CSV
        {
            get
            {
                return new BlobProperties()
                {
                    ContentTyp = "text/csv"
                };
            }
        }

        public string ContentTyp { get; set; }

        public IDictionary<string, string> MetaData { get; set; }

        public static BlobProperties ImageContentTypeFortExtension(string extension)
        {
            if (extension == null)
            {
                return new BlobProperties() { ContentTyp = "image/jpg" };
            }

            extension = extension.ToLower();

            if (extension.Contains("pdf"))
            {
                return new BlobProperties() { ContentTyp = "application/pdf" };
            }

            // default to image type
            return new BlobProperties() { ContentTyp = "image/" + extension };
        }
    }

    public static class BlobStorageHelper
    {

        public static string UploadFileToBlob(this AzureCloudBlobStorage store, Stream inputStream, string filename, BlobProperties properties = null)
        {
            if (inputStream != null && inputStream.Length > 0)
            {

                // Check blob does not already contain file
                if (store.Exists(filename))
                {
                    // Delete it
                    store.DeleteBlob(filename);
                }

                // Upload to blob
                if (store.SetBlobFromStream(filename, inputStream, properties))
                {
                    return filename;
                }

                return null;
            }
            return null;

        }
        public static bool EnsureBlobContainerExists(this CloudBlobClient blobClient, string containerName, BlobContainerPublicAccessType accessType = BlobContainerPublicAccessType.Off)
        {
            if (!containerName.All(_ => char.IsLower(_) || char.IsDigit(_) || _ == '-'))
                throw new ApplicationException("Azure container names must contain digits, lowercase letters, and dashes only.");
            if (containerName.StartsWith("-"))
                throw new ApplicationException("Azure container names must start with digit or lowercase letter.");

            var container = blobClient.GetContainerReference(containerName);
            return container.CreateIfNotExists(accessType);

        }

    }

    public class AzureCloudBlobStorage
    {
        private static readonly HashSet<HttpStatusCode> GracefulFailCodes = new HashSet<HttpStatusCode>() {
            HttpStatusCode.NotFound,
            //HttpStatusCode.BadRequest
        };

        private readonly CloudBlobContainer _blobContainer;
        private readonly string _cdnUrl = "";
        //public AzureCloudBlobStorage(/*CloudBlobContainer blobContainer*/)
        public AzureCloudBlobStorage(string storageBlobContainer)
        {

            var connectionString = ConfigurationManager.ConnectionStrings["AzureWebJobsStorage"].ConnectionString;
            var cdnUrl = ConfigurationManager.AppSettings["AzureBlobUrl"];

            CloudStorageAccount account = CloudStorageAccount.Parse(connectionString);
            CloudBlobClient client = account.CreateCloudBlobClient();
            CloudBlobContainer cont = client.GetContainerReference(storageBlobContainer);

            this._blobContainer = cont;
            this._cdnUrl = cdnUrl;
        }

        public static ReturnType RetryQuery<ReturnType>(Func<ReturnType> action)
        {
            //see msdn doc for StorageClientException 
            var canRetry = true;
            while (canRetry)
            {
                try
                {
                    return action();
                }
                //catch (StorageClientException ex)
                catch (StorageException ex)
                {
                    var reqInfo = ex.RequestInformation;

                    if (reqInfo.ExtendedErrorInformation == null || !reqInfo.ExtendedErrorInformation.ErrorCode.Equals(
                        StorageErrorCodeStrings.InternalError.ToString(CultureInfo.InvariantCulture)))
                    {
                        canRetry = false;

                        if (!GracefulFailCodes.Contains((HttpStatusCode)reqInfo.HttpStatusCode))
                        {
                            Trace.TraceError("AzureStorageClient Error: {0}, Stack {1}", ex.Message, ex.StackTrace);
                        }
                    }
                    else
                    {
                        // Wait before retrying the operation
                        //Trace
                        Thread.Sleep(500);
                    }
                }
            }
            //Trace
            return default(ReturnType);
        }

        public bool CreateIfNotExists()
        {
            Func<bool> create = () => this._blobContainer.CreateIfNotExists();
            return RetryQuery(create);
        }

        public bool Delete()
        {
            Func<bool> delete = () => {
                this._blobContainer.Delete();
                return true;
            };
            return RetryQuery(delete);
        }

        public bool Exists()
        {
            Func<bool> exists = () => {
                this._blobContainer.FetchAttributes();
                return true;
            };
            return RetryQuery(exists);
        }

        public bool SetContainerPermissions(BlobContainerPublicAccessType PublicAccess)
        {
            Func<bool> SetPerms = () => {
                this._blobContainer.SetPermissions(new BlobContainerPermissions() { PublicAccess = PublicAccess });
                return true;
            };
            return RetryQuery(SetPerms);
        }

        private static void UpdateFromProperties(ICloudBlob blob, BlobProperties properties, bool update = false)
        {
            if (properties == null)
            {
                return;
            }

            if (properties.ContentTyp != null)
            {
                blob.Properties.ContentType = properties.ContentTyp;
                if (update)
                {
                    blob.SetProperties();
                }
            }

            if (properties.MetaData != null)
            {
                foreach (var metaData in properties.MetaData)
                {
                    blob.Metadata.Remove(metaData.Key);
                    blob.Metadata.Add(metaData.Key, metaData.Value);
                }

                if (update)
                {
                    blob.SetMetadata();
                }
            }
        }

        #region Implementation of BlobStorageInterface

        public byte[] GetBlob(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return null;
            }

            Func<byte[]> getBlob = () => {
                var ms = new MemoryStream();
                this._blobContainer.GetBlockBlobReference(id).DownloadToStream(ms);
                return ms.ToArray();
            };

            //.DownloadByteArray();
            return RetryQuery(getBlob);
        }

        public bool GetToStream(string id, Stream s)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return false;
            }

            Func<bool> getBlob = () => {
                this._blobContainer.GetBlockBlobReference(id).DownloadToStream(s);
                return true;
            };
            return RetryQuery(getBlob);
        }

        public Uri GetBlobUri(string id, bool cdnUri = true)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return null;
            }

            var ret = this._blobContainer.GetBlockBlobReference(id).Uri;
            return (!cdnUri || string.IsNullOrWhiteSpace(this._cdnUrl)) ? ret : new Uri(this._cdnUrl + ret.PathAndQuery);
        }


        public bool Exists(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return false;
            }

            Func<bool> getBlob = () => this._blobContainer.GetBlockBlobReference(id).Exists();
            return RetryQuery(getBlob);
        }

        public BlobProperties GetBlobProperties(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return null;
            }

            Func<BlobProperties> getProperties =
                () => {
                    var blob = this._blobContainer.GetBlockBlobReference(id);
                    blob.FetchAttributes();
                    return new BlobProperties()
                    {
                        MetaData = new Dictionary<string, string>(blob.Metadata),
                        ContentTyp = blob.Properties.ContentType
                    };
                };

            return RetryQuery(getProperties);
        }

        public bool SetBlob(string id, byte[] bytes, BlobProperties properties = null)
        {
            var ms = new MemoryStream(bytes);
            return this.SetBlobFromStream(id, ms, properties);
        }

        public bool SetBlobFromStream(string id, Stream stream, BlobProperties properties = null)
        {
            Func<bool> setBlob =
                () => {
                    var blob = this._blobContainer.GetBlockBlobReference(id);
                    UpdateFromProperties(blob, properties);
                    blob.UploadFromStream(stream);
                    return true;
                };
            return RetryQuery(setBlob);
        }

        public bool SetBlobProperties(string id, BlobProperties properties)
        {
            Func<bool> setBlob =
                () => {
                    var blob = this._blobContainer.GetBlockBlobReference(id);
                    UpdateFromProperties(blob, properties, true);
                    return true;
                };
            return RetryQuery(setBlob);
        }

        public bool DeleteBlob(string id)
        {
            Func<bool> deleteBlob = () =>
                this._blobContainer.GetBlockBlobReference(id).DeleteIfExists();
            return RetryQuery(deleteBlob);
        }

        public int BlobCount
        {
            get
            {
                Func<int> blobCount = () => this._blobContainer.ListBlobs().Count();
                return RetryQuery(blobCount);
            }
        }


        public IEnumerable<IListBlobItem> BlobList
        {
            get
            {
                Func<IEnumerable<IListBlobItem>> blobCount = () => this._blobContainer.ListBlobs();
                return RetryQuery(blobCount);
            }
        }

        #endregion
    }
}
