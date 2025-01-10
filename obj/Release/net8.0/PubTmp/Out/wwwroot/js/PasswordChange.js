
$(function () {
    $("#passwordChangeForm").on("submit", function (event) {
        event.preventDefault(); // Prevent form from reloading the page

        // Collect form data
        const formData = {
            username: $("#username").val(),
            oldPassword: $("#oldPassword").val(),
            newPassword: $("#newPassword").val()
        };

        // Send data via AJAX
        $.ajax({
            url: "/PasswordChange", // it will be inside ManageUser controller
            type: "POST",
            contentType: "application/json",
            data: JSON.stringify(formData), // Convert form data to JSON string
            success: function (data) {
                // Handle responses based on the specific messages from server
                if (data.success) {
                    Swal.fire({
                        icon: 'success',
                        title: 'Password Changed',
                        text: data.message || 'Your password has been updated successfully!',
                        confirmButtonColor: '#3085d6',
                        confirmButtonText: 'OK'
                    }).then(() => {
                        // Optionally, close modal after success
                        $("#passwordChangeModal").modal('hide');
                        $("#passwordChangeForm")[0].reset(); // Clear form inputs
                    });
                } else {
                    // Optionally, close modal after success

                    $("#passwordChangeForm")[0].reset(); // Clear form inputs
                    Swal.fire({
                        icon: 'error',
                        title: 'Error',
                        text: data.message || 'Failed to change password. Please try again.',
                        confirmButtonColor: '#d33',
                        confirmButtonText: 'Retry'
                    });
                }
            },
            error: function (xhr, status, error) {
                // Check if there is a BadRequest (400) error, typically returned from the server
                // Optionally, close modal after success

                $("#passwordChangeForm")[0].reset(); // Clear form inputs
                let errorMessage = 'An error occurred. Please try again later.';

                // If it's a 400 error (BadRequest), show the specific error message from the server
                if (xhr.status === 400) {
                    // If the response body contains a message (like password validation issue)
                    errorMessage = xhr.responseText || 'Choose a strong password, like "Test@123".';
                } else if (xhr.responseJSON && xhr.responseJSON.message) {
                    // If the error response contains a custom message from the server
                    errorMessage = xhr.responseJSON.message;
                }

                Swal.fire({
                    icon: 'error',
                    title: 'Error',
                    text: errorMessage,
                    confirmButtonColor: '#d33',
                    confirmButtonText: 'Close'
                });
            }
        });
    });
});
