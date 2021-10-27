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

    $('.tablesorter thead tr th').on('click', function () {
        var icon = $(this).find('i');

        $('.tablesorter thead tr th i').each((i, e) => {
            //$(e).toggleClass('active');
            if ($(icon).closest('th').index() != i) {
                $(e).removeClass('active');
                $(e).removeClass('fa-sort-asc');
                $(e).removeClass('fa-sort-desc');
                $(e).addClass('fa-sort');
            }
        });

        if (icon.length != 0) {
            //Tiene icono entonces cambiarlo
            if ($(icon).hasClass('active')) {
                if ($(icon).attr('class').indexOf('fa-sort-desc') != -1) {
                    //el icono se encuentra ascendente entonces cambiarlo a descendente.
                    $(icon).removeClass('fa-sort-desc');
                    $(icon).addClass('fa-sort-asc');
                } else if ($(icon).attr('class').indexOf('fa-sort-asc') != -1) {
                    $(icon).removeClass('fa-sort-asc');
                    $(icon).addClass('fa-sort-desc');
                }
            } else {
                $(icon).removeClass('fa-sort')
                $(icon).addClass('active');
                $(icon).addClass('fa-sort-desc');
            }
        }
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

function returnFormatThousands(value) {
    return value.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ",");
}