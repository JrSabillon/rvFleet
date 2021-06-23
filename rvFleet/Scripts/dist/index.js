var width;

$(document).ready(function () {
    var menu_btn = document.querySelector("#menu-btn");
    var sidebar = document.querySelector("#sidebar");
    var container = document.querySelector(".my-container");
    menu_btn.addEventListener("click", () => {
        sidebar.classList.toggle("active-nav");
        container.classList.toggle("active-cont");
    });

    ViewLayout();

    width = $(window).width();

    $(window).on('resize', function () {
        ViewLayout();
    });

    $('input[type=datetime]').datepicker({
                dateFormat: "dd/M/yy",
                changeMonth: true,
                changeYear: true
            });
});

function ViewLayout() {
    if (width != $(window).width()) {
        width = $(window).width();

        if (window.innerWidth < 799) {
            $('#sidebar').removeClass('active-nav');
            $('#div-content').removeClass('active-cont');
        } else {
            $('#sidebar').addClass('active-nav');
            $('#div-content').addClass('active-cont');
        }
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

function formatThousands(element) {
    element.value = element.value.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ",");
}