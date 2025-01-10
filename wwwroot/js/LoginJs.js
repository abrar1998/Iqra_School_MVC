//$(function () {

//    const formOpenBtn = document.querySelector("#form-open"),
//        home = document.querySelector(".home"),
//        formContainer = document.querySelector(".form_container"),
//        formCloseBtn = document.querySelector(".form_close"),
//        signupBtn = document.querySelector("#signup"),
//        loginBtn = document.querySelector("#login"),
//        pwShowHide = document.querySelectorAll(".pw_hide");

//    formOpenBtn.addEventListener("click", () => home.classList.add("show"));
//    formCloseBtn.addEventListener("click", () => home.classList.remove("show"));

//    pwShowHide.forEach((icon) => {
//        icon.addEventListener("click", () => {
//            let getPwInput = icon.parentElement.querySelector("input");
//            if (getPwInput.type === "password") {
//                getPwInput.type = "text";
//                icon.classList.replace("uil-eye-slash", "uil-eye");
//            } else {
//                getPwInput.type = "password";
//                icon.classList.replace("uil-eye", "uil-eye-slash");
//            }
//        });
//    });

//    signupBtn.addEventListener("click", (e) => {
//        e.preventDefault();
//        formContainer.classList.add("active");
//    });

//    loginBtn.addEventListener("click", (e) => {
//        e.preventDefault();
//        formContainer.classList.remove("active");
//    });

//});

$(function () {
    const formOpenBtn = document.querySelector("#form-open"),
        home = document.querySelector(".home"),
        formContainer = document.querySelector(".form_container"),
        formCloseBtn = document.querySelector(".form_close"),
        signupBtn = document.querySelector("#signup"),
        loginBtn = document.querySelector("#login"),
        pwShowHide = document.querySelectorAll(".pw_hide");

    if (formOpenBtn) {
        formOpenBtn.addEventListener("click", () => home.classList.add("show"));
    }

    if (formCloseBtn) {
        formCloseBtn.addEventListener("click", () => home.classList.remove("show"));
    }

    pwShowHide.forEach((icon) => {
        icon.addEventListener("click", () => {
            let getPwInput = icon.parentElement.querySelector("input");
            if (getPwInput.type === "password") {
                getPwInput.type = "text";
                icon.classList.replace("uil-eye-slash", "uil-eye");
            } else {
                getPwInput.type = "password";
                icon.classList.replace("uil-eye", "uil-eye-slash");
            }
        });
    });

    if (signupBtn) {
        signupBtn.addEventListener("click", (e) => {
            e.preventDefault();
            formContainer.classList.add("active");
        });
    }

    if (loginBtn) {
        loginBtn.addEventListener("click", (e) => {
            e.preventDefault();
            formContainer.classList.remove("active");
        });
    }

    // Adding event listener for button click to show loader
    $('#btnSubmit').click(function () {
        $('#loader').show();
    });

    $('#btnSignup').click(function () {
        $('#loader').show();
    });
});
