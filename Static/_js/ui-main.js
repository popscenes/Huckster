jQuery.noConflict();
(function($) {
"use strict";

	/*********
		RESTAURANT LISTING ORDER MENU HOVER FUNCTIONS
	*********/	
	$(document).ready(function() {
	
		//variables
		var list = $('.order-main table tbody tr');
		var qty = $('.order-item-hover');
		var del = $('.order-item-delete');
		
		//functions
		var sPath = window.location.pathname;
		var sPage = sPath.substring(sPath.lastIndexOf('/') + 1);
		//console.log(sPage);
		
		// if sPage = the restaurant listing page do something else do nothing.
		// change this value to suit the url string for whatever page the restaurant listing ends up on.
		if (sPage === 'restaurant-listing.html' ) {
			
			list.on('mouseover', function(){
		
			$(this).find(qty).addClass('show');
			$(this).find(del).addClass('show');
			
			});
			list.on('mouseout', function(){
				$(this).find(qty).removeClass('show');
				$(this).find(del).removeClass('show');
			});
			
		} else {
			return;	
		}
		
		
	});
	
	/*********
		STEP REVEALS
	*********/
	$(document).ready(function() {
	//variables
				
				var two = $('#step-two');
				var three = $('#step-three');
				var four = $('.credit-container');
				var btnOne = $('#chkBtnOne');
				var btnTwo = $('#chkBtnTwo');
				var btnPaypal = $('#paypal-btn');
				var btnCredit = $('#credit-btn');
		
		// make steps 2 + 3 + 4 disappear
				two.addClass('unstep');
				three.addClass('unstep');
				four.addClass('unstep');
				
				// make step 2 appear
				btnOne.on('click', function(e) {
					e.preventDefault(); // prevents the button from submitting the form
					two.removeClass('unstep');
					$(window).scrollTo(two, {duration: 800}); //scroll to this section on the page
				});
				
				// make step 3 appear
				
				
				btnTwo.on('click', function(e) {
					e.preventDefault(); // prevents the button from submitting the form
					three.removeClass('unstep');
					$(window).scrollTo(three, {duration: 800}); //scroll to this section on the page
				});
				
				// make step 4 appear
				btnCredit.on('click', function(e) {
					e.preventDefault(); 
					four.removeClass('unstep');
					$(window).scrollTo(four, {duration: 800}); //scroll to this section on the page
					btnCredit.addClass('clicked');
				});
				
				
	
	});
	
	
	
	
})(jQuery);