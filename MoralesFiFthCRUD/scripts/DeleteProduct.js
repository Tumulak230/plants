jQuery(document).ready(function () {
    $('.deletebtn').click(function () {
        var productId = $(this).data('product-id');
        var deleteUrl = $(this).data('delete-url');
        var cardToRemove = $(this).closest('.col-md-4');
        // Display confirmation prompt before deleting
        if (confirm('Are you sure you want to delete this product?')) {
            $.ajax({
                url: deleteUrl,
                type: 'POST',
                data: { productId: productId }, 
                success: function (result) {
                    // Show success message or update UI
                    alert('Product deleted successfully.');
                    location.reload(); // Reload the page for simplicity
                },
                error: function (xhr, status, error) {
                    // Display error message
                    alert('An error occurred while deleting the product.');
                    console.log(xhr.responseText); // Log detailed error to console
                }
            });
        }
    });
});


    