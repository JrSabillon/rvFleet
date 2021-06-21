$(document).ready(function () {
    var menu_btn = document.querySelector("#menu-btn");
    var sidebar = document.querySelector("#sidebar");
    var container = document.querySelector(".my-container");
    menu_btn.addEventListener("click", () => {
        sidebar.classList.toggle("active-nav");
        container.classList.toggle("active-cont");
    });

    ViewLayout();

    $(window).on('resize', function () {
        ViewLayout();
    });
});

function ViewLayout() {
    if (window.innerWidth < 799) {
        $('#sidebar').removeClass('active-nav');
        $('#div-content').removeClass('active-cont');
    } else {
        $('#sidebar').addClass('active-nav');
        $('#div-content').addClass('active-cont');
    }
}

function isLoading(load) {
    if (load) {
        $('#modal-loading').modal({
            backdrop: 'static'
        });
        return;
    }

    $('#modal-loading').modal('hide');
}