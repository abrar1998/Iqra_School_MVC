$(function () {
    //manage category start
    // Add photo category
    $('#submitBtn').on('click', function () {
        var form = $('#catForm')[0];

        // Validate the form
        if (form.checkValidity()) {
            $('#loader').show();
            let formdata = new FormData(form);
            $.ajax({
                url: '/Gallery/AddPhotoCategory',
                type: 'POST',
                data: formdata,
                processData: false, // Required for FormData
                contentType: false, // Required for FormData
                success: function (response) {
                    $('#loader').hide();
                    // Handle the success response
                    if (response.isSuccess && response.status == 1) {
                        //swal("Thank You!", response.message, "success", {
                        //    button: "Ok!"
                        //});
                        swal("Thank You!", response.message, "success", {
                            button: "Ok!"
                        }).then(() => {

                            window.location.reload(); // Reload the page after clicking "Ok"
                        });
                    } else {
                        // Handle the error response
                        swal("Error", response.message, "error", {
                            button: "Ok!"
                        });
                    }
                },
                error: function (xhr, status, error) {
                    // Handle AJAX error
                    $('#loader').hide();
                    swal("Error", "Something went wrong while submitting the notice.", "error", {
                        button: "Ok!"
                    });
                }
            });
        } else {
            $('#loader').hide();
            // Trigger form validation
            form.classList.add('was-validated');
        }
    });

    //delete

    // Bind delete button click event dynamically
    $(document).on('click', '.btn-delete-category', function () {
        var catId = $(this).data('id');
        deleteCategory(catId);
    });
    function deleteCategory(catId) {
        swal({
            title: "Are you sure?",
            text: "Once deleted, you will not be able to recover this Slide!",
            icon: "warning",
            buttons: true,
            dangerMode: true,
        })
            .then((willDelete) => {
                if (willDelete) {
                    $('#loader').show();
                    $.ajax({
                        type: 'POST',
                        url: '/Gallery/DeletePhotoCategory',
                        data: { id: catId },
                        success: function (response) {
                            $('#loader').hide();
                            if (response.isSuccess && response.status === 1) {
                                // Remove the row from the table
                                $('#slide_' + catId).remove();

                                swal("Success!", "Photo Category has been deleted.", {
                                    icon: "success",
                                });
                            } else if (response.Status === 0) {
                                // Display error message from the server
                                swal("Error!", "Failed to delete the category.", {
                                    icon: "error",
                                });
                            } else {
                                // Handle unexpected status
                                swal("Technical Error!", response.error, {
                                    icon: "error",
                                });
                            }
                        },
                        error: function () {
                            $('#loader').hide();
                            swal("Error!", "An error occurred while processing your request.", {
                                icon: "error",
                            });
                        },
                    });
                } else {
                    $('#loader').hide();
                    swal("Canceled", "Slide deletion has been canceled.", {
                        icon: "info",
                    });
                }
            });
    }

    // Handle edit button click
    $(document).on('click', '.btn-edit-category', function () {
        var categoryId = $(this).data('id');

        // Fetch category details using AJAX
        $.ajax({
            url: '/Gallery/GetPhotoCategory',
            type: 'GET',
            data: { id: categoryId },
            success: function (response) {
                if (response.isSuccess && response.status === 1) {
                    // Populate the form fields with the fetched data
                    $('#editCategoryId').val(response.data.id);
                    $('#pcategoryName').val(response.data.name);
                    //$('#categoryDate').val(response.data.date);
                    $('#pexistingThumbnail').html(`
                        <img src="${response.data.thumbnailUrl}" alt="Existing Thumbnail" class="img-fluid rounded" style="max-width: 150px;">
                    `);

                    // Show the modal
                    $('#editCategoryModal').modal('show');
                } else {
                    swal("Error", "Failed to fetch category details.", "error");
                }
            },
            error: function () {
                swal("Error", "An error occurred while fetching category details.", "error");
            }
        });
    });

    // Handle form submission
    $('#editCategoryForm').on('submit', function (e) {
        e.preventDefault();

        // Create FormData object
        var formData = new FormData(this);
        $('#loader').show();

        // Send AJAX request to update the category
        $.ajax({
            url: '/Gallery/EditPhotoCategory',
            type: 'POST',
            data: formData,
            processData: false,
            contentType: false,
            success: function (response) {
                $('#loader').hide();
                if (response.isSuccess && response.status === 1) {
                    swal("Success", "Photo category updated successfully.", "success").then(() => {
                        $('#editCategoryModal').modal('hide'); // Hide the modal
                        window.location.reload(); // Reload the page to reflect changes
                    });
                } else {
                    swal("Error", response.message || "Failed to update the photo category.", "error");
                }
            },
            error: function () {
                $('#loader').hide();
                swal("Error", "An error occurred while updating the photo category.", "error");
            }
        });
    });

    // Handle form submission
    $('#editCategoryForm').on('submit', function (e) {
        e.preventDefault();

        // Create FormData object
        var formData = new FormData(this);
        $('#loader').show();

        // Send AJAX request to update the category
        $.ajax({
            url: '/Gallery/EditPhotoCategory',
            type: 'POST',
            data: formData,
            processData: false,
            contentType: false,
            success: function (response) {
                $('#loader').hide();
                if (response.isSuccess && response.status === 1) {
                    swal("Success", "Photo category updated successfully.", "success").then(() => {
                        $('#editCategoryModal').modal('hide'); // Hide the modal
                        window.location.reload(); // Reload the page to reflect changes
                    });
                } else {
                    swal("Error", response.message || "Failed to update the photo category.", "error");
                }
            },
            error: function () {
                $('#loader').hide();
                swal("Error", "An error occurred while updating the photo category.", "error");
            }
        });
    });

    //manage category send


    //manage images start, for images section only

    $('#submitImg').on('click', function () {
        var form = $('#imageForm')[0];

        // Validate the form
        if (form.checkValidity()) {
            $('#loader').show();
            let formdata = new FormData(form);
            $.ajax({
                url: '/Gallery/AddPhoto', //gallery controllers AddPhoto method
                type: 'POST',
                data: formdata,
                processData: false, // Required for FormData
                contentType: false, // Required for FormData
                success: function (response) {
                    $('#loader').hide();
                    // Handle the success response
                    if (response.isSuccess && response.status == 1) {
                        swal("Thank You!", response.message, "success", {
                            button: "Ok!"
                        }).then(() => {
                            // Reset the form after successful submission
                            form.reset(); // Resets the form fields to their initial values
                        });
                        //swal("Thank You!", response.message, "success", {
                        //    button: "Ok!"
                        //}).then(() => {
                        //    window.location.reload(); // Reload the page after clicking "Ok"
                        //});
                    } else {
                        // Handle the error response
                        swal("Error", response.message, "error", {
                            button: "Ok!"
                        });
                    }
                },
                error: function (xhr, status, error) {
                    // Handle AJAX error
                    $('#loader').hide();
                    swal("Error", "Something went wrong while submitting the notice.", "error", {
                        button: "Ok!"
                    });
                }
            });
        } else {
            $('#loader').hide();
            // Trigger form validation
            form.classList.add('was-validated');
        }
    });


    // Handle 'View Images' button click
    $('#viewImagesBtn').on('click', function () {
        // Hide the button
        $(this).hide();

        // Show the album dropdown
        $('#albumDropdown').show();
    });

    // Handle album selection change
    // Show the album dropdown when the "View Images" button is clicked
    $('#viewImagesBtn').on('click', function () {
        $('#photoSection').hide()
        $('#albumDropdown').show(); // Display the dropdown
        $('#imagesTable').hide();  // Hide the table if it's already shown
    });

    // Handle dropdown change event
    //add images dynamically to table
    $(document).on('change', '#albumSelect2', function () {
        var albumId = $(this).val();
        //console.log("Value selected: " + albumId);

        if (albumId) {
            $('#loader').show();
            // Make the AJAX call to fetch images under the selected album
            $.ajax({
                url: '/Gallery/GetImagesByAlbum', // Replace with your API endpoint
                type: 'GET',
                data: { albumId: albumId },
                success: function (response) {
                    if (response.success) {
                        if (response.status === 1) {
                            //console.log(response);

                            // Populate the table with image data
                            var tableBody = $('#imageTableBody');
                            tableBody.empty(); // Clear any existing rows
                            $('#imagesTable').show();

                            response.images.forEach(function (image, index) {
                                var row = `
                            <tr>
                                <td>${index + 1}</td>
                                <td>
                                    <img src="${image.imageUrl}" alt="Image" class="img-thumbnail" style="width: 90px; height: 80px;">
                                </td>
                                <td>${image.uploadDate}</td>
                                <td>
                                <button class="btn text-primary mt-2 view-image-btn" title="View Image" data-image-url="${image.imageUrl}">
                                    <i class="fas fa-eye"></i> View
                                </button>
                                </td>
                                <td>
                                  <button class="btn btn-sm text-danger delete-image-btn" data-id="${image.id}" title="Delete">
                                       <i class="fas fa-trash-alt"></i>
                                   </button>
                                </td>
                            </tr>`;
                                tableBody.append(row);
                            });
                            $('#loader').hide();
                        } else if (response.status === 0) {
                            $('#loader').hide();
                            $('#imagesTable').hide();
                            swal("Info", response.message || "No images found for the selected album.", "info", {
                                button: "Ok!"
                            });
                            $('#imagesTable').hide(); // Hide table if no images are found
                        }
                    }
                    else if (!response.success && response.status === 0) {
                        $('#loader').hide();
                        $('#imagesTable').hide();
                        swal("Info", response.message || "No images found for the selected album.", "info", {
                            button: "Ok!"
                        });
                        $('#imagesTable').hide(); // Hide table if no images are found
                    }
                    else {
                        $('#imagesTable').hide();
                        $('#loader').hide();
                        // Handle unsuccessful response
                        swal("Error", response.message || "Failed to fetch images for the selected album.", "error", {
                            button: "Ok!"
                        });
                    }
                },
                error: function (xhr, status, error) {
                    // Handle AJAX error
                    $('#loader').hide();
                    console.error("AJAX Error:", status, error);
                    swal("Error", "Something went wrong while fetching the images. Please try again later.", "error", {
                        button: "Ok!"
                    });
                }
            });
        } else {
            // No album selected
            swal("Warning", "Please select an album.", "warning", {
                button: "Ok!"
            });
        }
    });

    //open image in another tag
    $(document).on('click', '.view-image-btn', function () {
        var imageUrl = $(this).data('image-url');
        window.open(imageUrl, '_blank'); // Open the image in a new tab
    });



    //delete image
    $(document).on('click', '.delete-image-btn', function () {
        var imageId = $(this).data('id'); // Get the image ID from the button's data attribute

        // Confirm deletion with the user
        swal({
            title: "Are you sure?",
            text: "Once deleted, you will not be able to recover this image!",
            icon: "warning",
            buttons: true,
            dangerMode: true,
        }).then((willDelete) => {
            if (willDelete) {
                $('#loader').show();
                // Perform AJAX request to delete the image
                $.ajax({
                    url: '/Gallery/DeletePhoto', // API endpoint for deletion
                    type: 'POST', // Use POST or DELETE method as appropriate
                    data: { id: imageId }, // Pass the image ID
                    success: function (response) {
                        $('#loader').hide();
                        if (response.isSuccess && response.status == 1) {
                            // Remove the deleted image's row from the table
                            $(`button[data-id="${imageId}"]`).closest('tr').remove();
                            swal("Success", response.Message, "success");
                        } else {
                            swal("Error", response.Message || "Failed to delete the image.", "error");
                        }
                    },
                    error: function () {
                        $('#loader').hide();
                        swal("Error", "Something went wrong while deleting the image.", "error");
                    }
                });
            }
        });
    });




    //manage images end

    //manage youtube section
    $('#SubmitYtBtn').on('click', function () {
        var form = $('#ytForm')[0];

        // Validate the form
        if (form.checkValidity()) {
            $('#loader').show();
            let formdata = new FormData(form);
            $.ajax({
                url: '/Gallery/AddYoutubeLink',
                type: 'POST',
                data: formdata,
                processData: false, // Required for FormData
                contentType: false, // Required for FormData
                success: function (response) {
                   
                    $('#loader').hide();
                    // Handle the success response
                    if (response.isSuccess && response.status == 1) {
                        //swal("Thank You!", response.message, "success", {
                        //    button: "Ok!"
                        //});
                         $('#createYouTubeModal').hide();
                        swal("Thank You!", response.message, "success", {
                            button: "Ok!"
                        }).then(() => {

                            window.location.reload(); // Reload the page after clicking "Ok"
                        });
                    } else if (!response.isSuccess && response.status == -1)
                    {
                        // Handle the error response
                        swal("Error", response.error, "error", {
                            button: "Ok!"
                        });
                    }
                    else if (response.isSuccess && response.status == 0) {
                        // Handle the error response
                        swal("Info", response.message || "No information available", "info", {
                            button: "Ok!"
                        });;
                    }
                    else {
                        // Handle the error response
                        swal("Error", response.message, "error", {
                            button: "Ok!"
                        });
                    }
                },
                error: function (xhr, status, error) {
                    $('#createYouTubeModal').hide();
                    // Handle AJAX error
                    $('#loader').hide();
                    swal("Error", "Something went wrong while submitting the notice.", "error", {
                        button: "Ok!"
                    });
                }
            });
        } else {
            $('#loader').hide();
            // Trigger form validation
            form.classList.add('was-validated');
        }
    });

    // Bind delete button click event dynamically
    $(document).on('click', '.delete-youtube-button', function () {
        var yId = $(this).data('id');
        deleteYoutubeLink(yId);
    });
    function deleteYoutubeLink(yId) {
        swal({
            title: "Are you sure?",
            text: "Once deleted, you will not be able to recover this Video Link!",
            icon: "warning",
            buttons: true,
            dangerMode: true,
        })
            .then((willDelete) => {
                if (willDelete) {
                    $('#loader').show();
                    $.ajax({
                        type: 'POST',
                        url: '/Gallery/DeleteYoutubeVideoLink',
                        data: { id: yId },
                        success: function (response) {
                            $('#loader').hide();
                            if (response.isSuccess && response.status === 1) {
                                // Remove the row from the table
                                $('#slide_' + yId).remove();

                                swal("Success!", "Link deleted! ", {
                                    icon: "success",
                                });
                            } else if (response.Status === 0) {
                                // Display error message from the server
                                swal("Error!", response.message, {
                                    icon: "error",
                                });
                            } else {
                                // Handle unexpected status
                                swal("Technical Error!", response.error, {
                                    icon: "error",
                                });
                            }
                        },
                        error: function () {
                            $('#loader').hide();
                            swal("Error!", "An error occurred while processing your request.", {
                                icon: "error",
                            });
                        },
                    });
                } else {
                    $('#loader').hide();
                    swal("Canceled", "link deletion has been canceled.", {
                        icon: "info",
                    });
                }
            });
    }


});
