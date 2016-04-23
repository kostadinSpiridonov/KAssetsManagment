$(function () {
    // This will make every element with the class "date-picker" into a DatePicker element
    $('.date-picker').datepicker( );


})

function SetNowDate() {
    var currentDate = new Date()
    var day = currentDate.getDate()
    var month = currentDate.getMonth()
    var year = currentDate.getFullYear()
    $('.date-picker').datepicker("setDate", new Date(year, month, day));
}