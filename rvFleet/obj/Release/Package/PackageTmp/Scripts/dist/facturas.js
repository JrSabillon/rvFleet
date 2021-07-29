
var detailId = '', updatingRow = null;
var detail_json_array = [];
var json_factura = {
    FacCodigoOrden      : 0,
    FacCodigoProveedor  : '',
    FacUsuarioPago      : '',
    FacFechaOrden       : '',
    FacFechaFactura     : '',
    FacNumeroFactura    : '',
    FacComentario       : '',
    FacAplicaImpuesto   : 0,
    detallefactura      : detail_json_array
};

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

    $('#input-check-imp').on('click', function () {
        if ($(this).is(':checked')) {
            //Calcular total + impuesto del total actual
            var subtotal = parseFloat($('#span-caption-total').text().replace(',', ''));
            var total = subtotal * 0.15 + subtotal;

            $('#span-caption-total').text(total.toFixed(2).replace(/\d(?=(\d{3})+\.)/g, '$&,'));
        } else {
            var total = 0;

            for (var i = 0; i < detail_json_array.length; i++) {
                total = total + (detail_json_array[i].DetValor * 1);
            }

            $('#span-caption-total').text(total.toFixed(2).replace(/\d(?=(\d{3})+\.)/g, '$&,'));
        }
    });

    ValidateRubroDetalle();
});

function AddDetail() {
    var vehicle, rubro, detail, amount, price, detailId, kilometraje;
    vehicle = $('#DetPlacaVehiculo option:selected');
    rubro = $('#select-rubro option:selected');
    detail = $('#select-rubroDetalle option:selected');
    amount = $('#DetCantidad').val();
    kilometraje = $('#DetKilometraje').val();
    price = $('#DetValor').val().replace(',', '');
    detailId = detail.val();

    if (kilometraje.trim() == '') {
        $('#DetKilometraje').addClass('is-invalid');
        $('#div-alert-message').removeClass('d-none');
        $('#label-alert-message').text('El kilometraje es obligatorio.');

        return;
    } else {
        $('#DetKilometraje').removeClass('is-invalid');
    }

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

    LoadJSON(vehicle.text(), rubro.val(), detail, amount, price, kilometraje.replace(',', ''));
    price = price * 1;
    $('#div-alert-message').addClass('d-none');
    var html = `<tr>
                        <td>${vehicle.text()}</td>
                        <td>${kilometraje}</td>
                        <td>${rubro.text()}</td>
                        <td>${detail}</td>
                        <td>${amount}</td>
                        <td>${price.toFixed(2).replace(/\d(?=(\d{3})+\.)/g, '$&,')}</td>
                        <td class="text-center" style="white-space:nowrap">
                            <button type="button" class="btn btn-warning btn-sm" onclick="editDetail('${vehicle.val()}', '${kilometraje}', '${rubro.val()}', '${detail}', '${detailId}', '${amount}', '${price.toFixed(2).replace(/\d(?=(\d{3})+\.)/g, '$&,')}', event)">
                                <i class="fa fa-pencil"></i>
                            </button>
                            <button type="button" class="btn btn-danger btn-sm delete-button">
                                <i class="fa fa-trash-o"></i>
                            </button>
                        </td>
                    </tr>`
    $('#tbody-details').append(html);

    $('#DetCantidad').val('1');
    $('#DetValor').val('');
    $('#textarea-description').val('');

    $("#select-rubroDetalle").val($("#select-rubroDetalle option:first").val()).trigger('change.select2');
    CalculateTotal();
}

function editDetail(vehicle, kilometraje, rubro, detail, detailID, amount, price, e) {
    detailId = detailID;
    $('#DetPlacaVehiculo').val(vehicle).trigger('change.select2');
    $('#select-rubro').val(rubro).trigger('change.select2');
    $('#DetKilometraje').val(returnFormatThousands(kilometraje));
    $('#DetCantidad').val(amount);
    $('#DetValor').val(price);

    if (detailId == 0) {
        $('#textarea-description').val(detail);
    }

    updatingRow = $(e.target).closest('tr').index();
    $('#button-updateDetail').removeClass('d-none');
    $('#button-saveDetail').addClass('d-none');
}

function UpdateDetail() {
    $('#table-detail tbody tr').eq(updatingRow).remove();
    detail_json_array.splice(updatingRow, 1);
    AddDetail();
    $('#DetCantidad').val('1');
    $('#DetValor').val('');
    $('#textarea-description').val('');
    $("#select-rubroDetalle").val($("#select-rubroDetalle option:first").val()).trigger('change.select2');
    $('#button-updateDetail').addClass('d-none');
    $('#button-saveDetail').removeClass('d-none');
    
    updatingRow = null;
}

function ValidateRubroDetalle() {
    if ($('#select-rubroDetalle').val() == "0") {
        $('#div-descriptionDetail').removeClass('d-none');
    } else {
        $('#div-descriptionDetail').addClass('d-none');
    }
}

function LoadJSON(vehiculo, rubro, detalle, cantidad, precio, kilometraje) {
    detail_json_array.push(
        {
            DetPlacaVehiculo: vehiculo,
            DetCodigoRubro: rubro,
            DetDescripcion: detalle,
            DetCantidad: cantidad,
            DetValor: precio,
            DetKilometraje: kilometraje
        }
    );
}

function CalculateTotal() {
    var total = 0;

    for (var i = 0; i < detail_json_array.length; i++) {
        total = total + (detail_json_array[i].DetValor * 1);
    }

    if ($('#input-check-imp').is(':checked')) {
        total = total * 0.15 + total;
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
    json_factura.FacComentario = $('#FacComentario').val();
    json_factura.FacAplicaImpuesto = $('#input-check-imp').is(':checked');
    json_factura.detallefactura = detail_json_array;

    if (json_factura.FacFechaOrden == '') {
        flag = false;
        $('#FacFechaOrden').addClass('is-invalid');
    } else {
        $('#FacFechaOrden').removeClass('is-invalid');
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

        if (detailId != '') {
            $('#select-rubroDetalle').val(detailId).trigger('change.select2');
            detailId = '';
        }
        ValidateRubroDetalle();
        isLoading(false);
    });
}

function loadNumeroFactura(url) {
    isLoading(true);

    var proveedor = $('#FacCodigoProveedor');
    $.get(url, { CodigoProveedor: proveedor.val() }, function (data) {
        if (data.status == 200) {
            $('#FacNumeroFactura').val(data.message);
        }

        isLoading(false);
    });
}

function VerifyVehicleRegistry() {
    if (detail_json_array.length > 0) {
        var CurrentVehicle = $('#DetPlacaVehiculo option:selected').text();

        $.each(detail_json_array, function (index, item) {
            if (item.DetPlacaVehiculo == CurrentVehicle) {
                $('#DetKilometraje').val(returnFormatThousands(item.DetKilometraje));
                return;
            }
        });
    }
}

function ValidateNumFactura(url) {
    $.get(url, { FacNumeroFactura: $('#FacNumeroFactura').val() }, function (data) {
        if (data.status == 200) {
            if (data.message == "Error") {
                $('#div-alert-message-factura').removeClass('d-none');
                $('#label-alert-message-factura').text('¡Atención! Este número de factura ya se encuentra registrado.');
                $('#FacNumeroFactura').addClass('is-invalid');
            } else {
                $('#div-alert-message-factura').addClass('d-none');
                $('#label-alert-message-factura').text('');
                $('#FacNumeroFactura').removeClass('is-invalid');
            }
        } else {
            alert(data.message);
        }
    });
}