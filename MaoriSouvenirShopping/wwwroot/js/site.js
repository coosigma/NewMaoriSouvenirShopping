// Write your JavaScript code.

function onUpdateCartComplete(ajax) {
    var res = ajax.responseJSON;
    var items = res.items;
    var new_string = "";
    items.forEach(function (item) {
        new_string += '<hr /><div class="row">';
        // item's id
        new_string += '<div class="col-sm-3">' +
            '<a asp-action="Details" asp-route-id="' + item.id + '">' + item.id + '</a></div>';
        // item's name
        new_string += '<div class="col-sm-3">' +
            '<a asp-action="Details" asp-route-id="' + item.id + '">' + item.name + '</a></div>';
        // item's quantity
        new_string += '<div class="col-sm-3">' + item.quantity+
            ' <a data-ajax="true" data-ajax-complete="onUpdateCartComplete" href="/ShoppingCart/RemoveFromCart/' + item.id +'"><span class="glyphicon glyphicon-remove-circle"></span></a></div>';
        // item's price
        new_string += '<div class="col-sm-3">' + item.price.toFixed(2) +'</div></div>';
    });
    $("#cart_items").html(new_string);
    $("#sub_total").html("$" + res.sub.toFixed(2));
    $("#gst_total").html("$" + res.gst.toFixed(2));
    $("#total_price").html("$" + res.total.toFixed(2));
}
