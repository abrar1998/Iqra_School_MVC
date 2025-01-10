$(function () {
    // Initially hide the notification container if no notifications
    $('.whats-new-container').css('visibility', 'hidden');
    $('.latest-news-section').css('visibility', 'hidden'); //also hide latest news slider initially

    let notifications = []; // Placeholder for fetched notifications
    let currentIndex = 0;

    // Function to Fetch Notifications from the Controller
    function fetchNotifications() {
        $.ajax({
            url: '/Home/GetNoticeList', // home controller and GetNoticeList Method
            method: 'GET',
            success: function (response) {
                notifications = response;
                //console.log("Fetched Notifications:", notifications);

                if (notifications && notifications.length > 0) {
                    $('.whats-new-container').css('visibility', 'visible'); // Show the container
                    updateNotification(); // Start displaying notifications
                } else {
                    $('#currentNotificationTitle').text("No new notifications.");
                }
            },
            error: function () {
                console.error("Error fetching notifications:");
                $('#notificationTitle').text("Failed to load notifications.");
            }
        });
    }

    // Function to Update the Notification Display
    function updateNotification() {
        const titleElement = $('#currentNotificationTitle');

        // Apply Fade-Out Effect
        titleElement.fadeOut(300, function () {
            if (notifications && notifications.length > 0) {
                const currentNotification = notifications[currentIndex];

                if (currentNotification.nid) {
                    titleElement.html(`<a href="/Home/GetNotice/${currentNotification.nid}" style="text-decoration: none; color: inherit;">
                                        ${currentNotification.title}</a>`);
                } else {
                    titleElement.html(`<a href="/Home/GetNotice/${currentNotification.nid}" style="text-decoration: none; color: inherit;">
                                        ${currentNotification.title}</a>`);
                }

                // Move to the Next Notification
                currentIndex = (currentIndex + 1) % notifications.length;
            } else {
                titleElement.text("No new notifications available.");
            }

            // Fade-In Effect
            titleElement.fadeIn(300);
        });
    }
    // Method to show the previous notification
    function showPreviousNotification() {
        if (currentIndex > 0) {
            currentIndex--;
        } else {
            currentIndex = notifications.length - 1;
        }
    }

    // Method to show the next notification
    function showNextNotification() {
        if (currentIndex < notifications.length - 1) {
            currentIndex++;
        } else {
            currentIndex = 0;
        }
    }

    // Rotate Notifications Every 5 Seconds
    setInterval(updateNotification, 5000);

    // Fetch Notifications on Page Load
    fetchNotifications();

    //latest news section start
    // notifications.js

    let notices = [];

    //youtube section 
    function fetchNews() {
        $.ajax({
            url: '/Home/GetLatestNewsList', // home controller and GetNoticeList Method
            method: 'GET',
            success: function (response) {
                notices = response;
                if (notices && notices.length > 0) {
                    $('.latest-news-section').css('visibility', 'visible'); // Show the container
                    renderNotices(); // Start displaying notifications
                } else {
                    // Handle case when no notifications are found
                    const container = document.getElementById("swiper-cards-container");
                    container.innerHTML = "<p>No notifications available.</p>";
                }
            },
            error: function () {
                console.error("Error fetching notifications:");
            }
        });
    }

    // Function to render notices into Swiper cards dynamically
    function renderNotices() {
        const container = document.getElementById("swiper-cards-container");
        container.innerHTML = ''; // Clear previous content

        notices.forEach(function (notice) {
            // Format the date (if needed)
            let formattedDate = notice.nDate; // Default to the original value
            if (notice.nDate) {
                // Try parsing and formatting the date if it's not already formatted
                formattedDate = new Date(notice.nDate).toLocaleDateString(); // You can customize the format here
            }
            const cardHTML = `
         <div class="card swiper-slide">
            <div class="image-content">
                <div class="card-image">
                    <img src="${notice.thumbNail}" alt="Thumbnail" class="card-img" loading="lazy">
                </div>
            </div>
            <div class="card-content">
                <h2 class="name">${notice.title}</h2>
                <p class="date">Date: ${formattedDate}</p> <!-- Displaying the formatted date -->
                <a href="/Home/GetNotice/${notice.nid}" target="_blank" class="button btn btn-sm">Read More</a>
            </div>
        </div>
        `;
            container.innerHTML += cardHTML;
        });

        // Reinitialize Swiper after content is added dynamically
        var swiper = new Swiper(".slide-content", {
            slidesPerView: 3,
            spaceBetween: 25,
            loop: true,
            centerSlide: true,
            fade: true,
            grabCursor: true,
            pagination: {
                el: ".swiper-pagination",
                clickable: true,
                dynamicBullets: true,
            },
            navigation: {
                nextEl: ".swiper-button-next",
                prevEl: ".swiper-button-prev",
            },
            breakpoints: {
                0: {
                    slidesPerView: 1,
                },
                520: {
                    slidesPerView: 2,
                },
                950: {
                    slidesPerView: 3,
                },
            },
        });
    }

    // Call fetchNotifications to load data when the document is ready
    fetchNews();

    //latest news section end


});
