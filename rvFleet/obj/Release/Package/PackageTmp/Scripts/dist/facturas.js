var detail_json_array = [];
var json_factura = {
    FacCodigoOrden: 0,
    FacCodigoProveedor: '',
    FacUsuarioPago: '',
    FacFechaOrden: '',
    FacFechaFactura: '',
    FacNumeroFactura: '',
    FacKilometraje: 0,
    FacComentario: '',
    detallefactura: detail_json_array
}

$(document).ready(function () {
    $('#FacCodigoProveedor').select2({
        theme: 'bootstrap4',
        width: '100%',
    });

    $('#DetPlacaVehiculo').select2({
        theme: 'bootstrap4',
        width: '100%',
    });

    $('#select-rubro').select2({
        theme: 'bootstrap4',
        width: '100%',
    });
    $('#select-rubroDetalle').select2({
        theme: 'bootstrap4',
        width: '100%',
    });

    $('#tbody-details').on('click', '.delete-button', function () {
        var index = $(this).closest('tr').index();
        $(this).closest('tr').remove();

        detail_json_array.splice(index, 1);
        CalculateTotal();
    });

    $('.decimal').keyup(function () {
        var val = $(this).val();
        if (isNaN(val)) {
            val = val.replace(/[^0-9\.]/g, '');
            if (val.split('.').length > 2)
                val = val.replace(/\.+$/, "");
        }
        $(this).val(val);
    });

    ValidateRubroDetalle();
});

function AddDetail() {
    var vehicle, rubro, detail, amount, price;
    vehicle = $('#DetPlacaVehiculo option:selected');
    rubro = $('#select-rubro option:selected');
    detail = $('#select-rubroDetalle option:selected');
    amount = $('#DetCantidad').val();
    price = $('#DetValor').val();

    if (price == '') {
        $('#DetValor').addClass('is-invalid');
        $('#div-alert-message').removeClass('d-none');
        $('#label-alert-message').text('El precio es obligatorio.');

        return;
    } else {
        $('#DetValor').removeClass('is-invalid');
    }

    if (amount == '') {
        $('#DetCantidad').addClass('is-invalid');
        $('#div-alert-message').removeClass('d-none');
        $('#label-alert-message').text('La cantidad es obligatoria.');

        return;
    } else {
        $('#DetCantidad').removeClass('is-invalid');
    }

    if (detail.val() == '0') {
        detail = $('#textarea-description').val();
        if (detail == '') {
            $('#textarea-description').addClass('is-invalid');
            $('#div-alert-message').removeClass('d-none');
            $('#label-alert-message').text('La descripción es obligatoria.');

            return;
        } else {
            $('#textarea-description').removeClass('is-invalid');
        }
        $('#div-descriptionDetail').addClass('d-none');
    } else {
        detail = detail.text();
    }

    price = price * 1;
    $('#div-alert-message').addClass('d-none');
    var html = `<tr>
                        <td>${vehicle.text()}</td>
                        <td>${rubro.text()}</td>
                        <td>${detail}</td>
                        <td>${amount}</td>
                        <td>${price.toFixed(2).replace(/\d(?=(\d{3})+\.)/g, '$&,')}</td>
                        <td class="text-center">
                            <button type="button" class="btn btn-danger btn-sm delete-button">
                                <i class="fa fa-trash-o"></i>
                            </button>
                        </td>
                    </tr>`
    $('#tbody-details').append(html);

    $('#DetCantidad').val('1');
    $('#DetValor').val('');
    $("#select-rubroDetalle").val($("#select-rubroDetalle option:first").val()).trigger('change.select2');
    LoadJSON(vehicle.text(), rubro.val(), detail, amount, price);
    CalculateTotal();
}

function ValidateRubroDetalle() {
    if ($('#select-rubroDetalle').val() == "0") {
        $('#div-descriptionDetail').removeClass('d-none');
    } else {
        $('#div-descriptionDetail').addClass('d-none');
    }
}

function LoadJSON(vehiculo, rubro, detalle, cantidad, precio) {
    detail_json_array.push({ DetPlacaVehiculo: vehiculo, DetCodigoRubro: rubro, DetDescripcion: detalle, DetCantidad: cantidad, DetValor: precio });
}

function CalculateTotal() {
    var total = 0;

    for (var i = 0; i < detail_json_array.length; i++) {
        total = total + detail_json_array[i].DetValor;
    }

    $('#span-caption-total').text(total.toFixed(2).replace(/\d(?=(\d{3})+\.)/g, '$&,'));
}

function createRequest(source, urlNew, urlEdit) {
    isLoading(true);
    var flag = true;
    json_factura.FacCodigoProveedor = $('#FacCodigoProveedor').val();
    json_factura.FacUsuarioPago = $('#FacUsuarioPago').val();
    json_factura.FacFechaOrden = $('#FacFechaOrden').val();
    json_factura.FacFechaFactura = $('#FacFechaFactura').val();
    json_factura.FacNumeroFactura = $('#FacNumeroFactura').val();
    json_factura.FacKilometraje = $('#FacKilometraje').val();
    json_factura.FacComentario = $('#FacComentario').val();
    json_factura.detallefactura = detail_json_array;

    if (json_factura.FacFechaOrden == '') {
        flag = false;
        $('#FacFechaOrden').addClass('is-invalid');
    } else {
        $('#FacFechaOrden').removeClass('is-invalid');
    }

    if (json_factura.FacKilometraje == '') {
        flag = false;
        $('#FacKilometraje').addClass('is-invalid');
    } else {
        $('#FacKilometraje').removeClass('is-invalid');
    }

    if (flag) {
        $('#div-alert-message-factura').addClass('d-none');
        if (json_factura.detallefactura.length == 0) {
            isLoading(false);
            alert('Favor agregar mínimo un detalle a la factura.');
        } else {
            if (source === "New") {
                $.post(urlNew, json_factura, function (data) {
                    if (data.status == 200) {
                        window.location.href = data.message;
                    } else {
                        isLoading(false);
                        alert(data.message);
                    }
                });
            } else {
                json_factura.FacCodigoOrden = $('#FacCodigoOrden').val();
                $.post(urlEdit, json_factura, function (data) {
                    if (data.status == 200) {
                        window.location.href = data.message;
                    } else {
                        isLoading(false);
                        alert(data.message);
                    }
                });
            }
        }
    } else {
        isLoading(false);
        $('#div-alert-message-factura').removeClass('d-none');
        alert('Favor llenar los campos obligatorios.');
        $('#label-alert-message-factura').text('Favor llenar los campos obligatorios.');
    }

}

function LoadRubroDetalles(url) {
    isLoading(true);
    $.get(url, { CodigoRubro: $('#select-rubro').val() }, function (data) {
        $('#select-rubroDetalle').html('');

        $.each(data, function (index, item) {
            $('#select-rubroDetalle').append(`<option value="${item.CodigoDetalle}">${item.NombreDetalle}</option>`);
        });

        ValidateRubroDetalle();
        isLoading(false);
    });
}