jQuery.noConflict();
(function($) {
"use strict";

	/*********
		CHECKOUT PAGE VALIDATION
	*********/
	
	
	$(document).ready(function(){
		
		(function(){ // Wrap inside IIFE
		document.forms.checkout.noValidate = true;		// Disable HTML5 validation
		document.forms.checkoutTwo.noValidate = true;
		
		//variables
				
				var two = $('#step-two');
				var three = $('#step-three');
				
		
		// make steps 2 + 3 disappear
				two.addClass('unstep');
				three.addClass('unstep');
				
				// make step 2 appear
				
				/*
				
				
				// make step 3 appear
				
				
				btnTwo.on('click', function(e) {
					e.preventDefault(); // prevents the button from submitting the form
					three.removeClass('unstep');
					$(window).scrollTo(three, {duration: 800}); //scroll to this section on the page
				});
				*/
		
		// -------------------------------------------------------------------------
		//  A) ANONYMOUS FUNCTION TRIGGERED BY THE SUBMIT EVENT
		// -------------------------------------------------------------------------
		$('.checkout-form#one, .checkout-form#two').on('submit', function(e) {	// When form is submitted
			var elements = this.elements;				// Collection of form controls
			var valid = {};								// Custom valid object
			var isValid;									// isValid: checks form controls
			var isFormValid;								// isFormValid: checks entire form
			var $this = $(this);
			
			console.log(elements.chkBtnOne);
			
			// PERFORM GENERIC CHECKS (calls functions outside the event handler)
			var i, l;
			for (i = 0, l = (elements.length - 1); i < l; i++) {
				
				// Next line calls validateRequired() & validateTypes()
				isValid = validateRequired(elements[i]) && validateTypes(elements[i]); 
				if (!isValid) {                    		// If it does not pass these two tests
				  showErrorMessage(elements[i]);   		// Show error messages
				  
				} else {                           		// Otherwise
				  removeErrorMessage(elements[i]); 		// Remove error messages
				}                                  		// End if statement
				valid[elements[i].id] = isValid;   		// Add element to the valid object
			}											// End for loop
			
			
			
				
			// DID IT PASS / CAN IT SUBMIT THE FORM?
			// Loop through valid object, if there are errors set isFormValid to false
			for (var field in valid) {          			// Check properties of the valid object
			  if (!valid[field]) {              			// If it is not valid
				isFormValid = false;            			// Set isFormValid variable to false
				break;                          		// Stop the for loop, an error was found
			  } else {
			  // Otherwise
			  	isFormValid = true;               			// The form is valid and OK to submit
			  } 
			}
		
		
			// If the form did not validate, prevent it being submitted
			if (!isFormValid) {                 			// If isFormValid is not true
			  e.preventDefault();               			// Prevent the form being submitted
			}
			
			
			if (isFormValid) {
				e.preventDefault();
				
			}
			
			
			
		}); // END: anonymous function triggered by the submit button
		
		
		// -------------------------------------------------------------------------
		// A:2) STEP REVEALS...
		// -------------------------------------------------------------------------
		
		
		
		/*
		$chkBtnOne.on( 'click', function() {
		
			
			
		});
		
		$chkBtnTwo.on('click', function() {
			// STEP REVEALS
			// make step 3 appear
			three.removeClass('unstep');
			$(window).scrollTo(three, {duration: 800}); //scroll to this section on the page
				
		});
		*/
		
		
		// -------------------------------------------------------------------------
		// B) FUNCTIONS FOR GENERIC CHECKS
		// -------------------------------------------------------------------------
	  
		// CHECK IF THE FIELD IS REQUIRED AND IF SO DOES IT HAVE A VALUE
		// Relies on isRequired() and isEmpty() both shown below, and setErrorMessage - shown later.
		function validateRequired(el) {
		  if (isRequired(el)) {                           // Is this element required?
			var valid = !isEmpty(el);                     // Is value not empty (true / false)?
			if (!valid) {                                 // If valid variable holds false
			  setErrorMessage(el,  'Field is required');  // Set the error message
			}
			return valid;                                 // Return valid variable (true or false)?
		  }
		  return true;                                    // If not required, all is ok
		}
	  
		// CHECK IF THE ELEMENT IS REQUIRED
		// It is called by validateRequired()
		function isRequired(el) {
		 return ((typeof el.required === 'boolean') && el.required) ||
		   (typeof el.required === 'string');
		}
	  
		// CHECK IF THE ELEMENT IS EMPTY (or its value is the same as the placeholder text)
		// HTML5 browsers do allow users to enter the same text as placeholder, but in this case users should not need to
		// It is called by validateRequired()
		function isEmpty(el) {
		  return !el.value || el.value === el.placeholder;
		}
	  
		// CHECK IF THE VALUE FITS WITH THE TYPE ATTRIBUTE
		// Relies on the validateType object (shown at end of IIFE)
		function validateTypes(el) {
		  if (!el.value) {return true;}                     // If element has no value, return true
														  // Otherwise get the value from .data()
		  var type = $(el).data('type') || el.getAttribute('type');  // OR get the type of input
		  if (typeof validateType[type] === 'function') { // Is the type a method of validate object?
			return validateType[type](el);                // If yes, check if the value validates
		  } else {                                        // If not
			return true;                                  // Return true because it cannot be tested
		  }
		}
		
		
	  
		// -------------------------------------------------------------------------
		// D) FUNCTIONS TO SET / GET / SHOW / REMOVE ERROR MESSAGES
		// -------------------------------------------------------------------------
	  
		function setErrorMessage(el, message) {
		  $(el).data('errorMessage', message);                 	// Store error message with element
		}
	  
		function getErrorMessage(el) {
		  return $(el).data('errorMessage') || el.title;       	// Get error message or title of element
		}
	  
		function showErrorMessage(el) {
		  var $el = $(el);                                     	// The element with the error
		  var errorContainer = $el.siblings('.error.message'); 	// Any siblings holding an error message
	  
		  if (!errorContainer.length) {                         	// If no errors exist with the element
			 // Create a <span> element to hold the error and add it after the element with the error
			 errorContainer = $('<span class="error message"></span>').insertAfter($el);
		  }
		  errorContainer.text(getErrorMessage(el));             // Add error message
		  
		  $el.addClass('input-error');							// Add red outline to input with error
		}
	  
		function removeErrorMessage(el) {
		  var $el = $(el);
		  var errorContainer = $(el).siblings('.error.message'); // Get the sibling of this form control used to hold the error message
		  errorContainer.remove();    							// Remove the element that contains the error message
		  
		  $el.removeClass('input-error');                           
		}
	  
	  
	  
		// -------------------------------------------------------------------------
		// E) OBJECT FOR CHECKING TYPES
		// -------------------------------------------------------------------------
	  
		// Checks whether data is valid, if not set error message
		// Returns true if valid, false if invalid
		var validateType = {
		  email: function (el) {                                 // Create email() method
			// Rudimentary regular expression that checks for a single @ in the email
			var valid = /[^@]+@[^@]+/.test(el.value);            // Store result of test in valid
			if (!valid) {                                        // If the value of valid is not true
			  setErrorMessage(el, 'Please enter a valid email'); // Set error message
			}
			return valid;                                        // Return the valid variable
		  },
		  number: function (el) {                                // Create number() method
			var valid = /^\d+$/.test(el.value);                  // Store result of test in valid
			if (!valid) {
			  setErrorMessage(el, 'Please enter a valid number');
			}
			return valid;
		  },
		  date: function (el) {                                  // Create date() method
																 // Store result of test in valid
			var valid = /^(\d{2}\/\d{2}\/\d{4})|(\d{4}-\d{2}-\d{2})$/.test(el.value);
			if (!valid) {                                        // If the value of valid is not true
			  setErrorMessage(el, 'Please enter a valid date');  // Set error message
			}
			return valid;                                        // Return the valid variable
		  }
		};
	
		
		}()); //End of IIFE
	});
	
	
	
})(jQuery);