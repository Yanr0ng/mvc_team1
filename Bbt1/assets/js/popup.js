var id;
function getid(pid) {
    id = pid;
    console.log(id);
}

$(document).ready(function () {
    $('.popmodal').click(function () {
        var url = $('#myModal').data('url');
        var url2 = url +"/"+ id;
        //var url2 = "/ProductM/ProductDetailPopup/" + id;
        $.get(url2, function (data) {
            $("#myModal").html(data);
            $("#myModal").modal('show');

        })
    });
})
var modal = document.getElementById("abc");
window.onclick = function (event) {
    if (event.target == modal) {
        modal.style.display = "none";
        $('#abc').data('dismiss') = "modal";
        $('#abc').modal('hide');
        $('#abc').hide();
    }
}

