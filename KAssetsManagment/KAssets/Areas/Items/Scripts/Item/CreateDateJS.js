$(function () {
    // This will make every element with the class "date-picker" into a DatePicker element
    $('.date-picker').datepicker( );
    var currentDate = new Date()
    var day = currentDate.getDate()
    var month = currentDate.getMonth() -1
    var year = currentDate.getFullYear()
    $('.date-picker').datepicker("setDate", new Date(year, month, day));
})