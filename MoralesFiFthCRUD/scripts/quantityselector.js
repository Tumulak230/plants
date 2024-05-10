function decrementQuantity() {
    var quantityInput = document.getElementById('productQuantity');
    var currentValue = parseInt(quantityInput.value);
    if (currentValue > 1) {
        quantityInput.value = currentValue - 1;
    }
}

function incrementQuantity() {
    var quantityInput = document.getElementById('productQuantity');
    var currentValue = parseInt(quantityInput.value);
    if (currentValue < 999) {
        quantityInput.value = currentValue + 1;
    }
}