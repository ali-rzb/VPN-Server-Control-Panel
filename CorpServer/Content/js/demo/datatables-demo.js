// Call the dataTables jQuery plugin
$(document).ready(function() {
    $('#dataTable').DataTable({
        "language": {
            "lengthMenu": "نمایش _MENU_ داده در هر صفحه",
            "zeroRecords": "هنوز اطلاعاتی ثبت نشده",
            "info": "نمایش صفحه _PAGE_ از _PAGES_ ",
            "infoEmpty": "هنوز اطلاعاتی ثبت نشده",
            "infoFiltered": "(filtered from _MAX_ total records)",
            "search": "جستوجو : ",
            "paginate": {
                "next" : "بعدی",
                "previous": "قبلی"
            }
        }
    });
});
