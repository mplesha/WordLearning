
   


$('span.field-validation-valid, span.field-validation-error').each(function () {
    $(this).addClass('help-block');
});

var $form = $('form');
var $validate = $form.validate();
var errorClass = "has-error";
$validate.settings.errorClass = errorClass;

var previousEPMethod = $validate.settings.errorPlacement;
$validate.settings.errorPlacement = $.proxy(function(error, inputElement) {
    if (previousEPMethod) {
        previousEPMethod(error, inputElement);
    }
    inputElement.parent().addClass(errorClass);
}, $form[0]);
 
 var previousSuccessMethod = $validate.settings.success;
 $validate.settings.success = $.proxy(function(error) {
     //we first need to remove the class, cause the unobtrusive success method removes the node altogether
     error.parent().parent().removeClass(errorClass);
     if (previousSuccessMethod) {
         previousSuccessMethod(error);
     }
 });

 $(function() {
     $('span.field-validation-valid, span.field-validation-error').each(function() {
         $(this).addClass('help-block');
     });

     var $form = $('form');
     var $validate = $form.validate();
     var errorClass = "has-error";
     $validate.settings.errorClass = errorClass;
     var previousEpMethod = $validate.settings.errorPlacement;
     $validate.settings.errorPlacement = $.proxy(function(error, inputElement) {
         if (previousEpMethod) {
             previousEpMethod(error, inputElement);
         }
         inputElement.parent().addClass(errorClass);
     }, $form[0]);

     var previousSuccessMethod = $validate.settings.success;
     $validate.settings.success = $.proxy(function(error) {
         //we first need to remove the class, cause the unobtrusive success method removes the node altogether
         error.parent().parent().removeClass(errorClass);
         if (previousSuccessMethod) {
             previousSuccessMethod(error);
         }
     });
 });

 function notEmpty(line) {
     var inpt = $(line).parent().find('span .hidden-this');
     if ($(line).val() != '') {
         inpt.attr('hidden', true);
     } else {
         inpt.removeAttr('hidden');
     }
 }

 //window.onload = function() {
 //    $('.selectpicker').selectpicker();
 //};
 $('.selectpicker').selectpicker();

