// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
$(function () {
    const url = $("#url-input");
    const developer = $("#developer-input");
    const password = $('#password');
    const visibility = $("#visibility");
    const visibility_icon = $("#visibility-icon");
    const pass_gen = $("#pass-gen");

    url.hide();
    developer.hide();

    $("#app-type").change(function () {
        const value = $(this).val();
        if (value === 'application'){
            url.hide();
            developer.hide();
        } else if (value === 'web'){
            url.show();
            developer.hide();
        } else if (value === 'game'){
            url.hide();
            developer.show();
        }
    })

    visibility.click((e) => {
        e.preventDefault();
        // alert(password.tagName.toLowerCase())
        // console.log("Test", password[0].localName);
        const type = password.attr("type");
        const cls = visibility_icon.attr("class");
        // alert(type);
        password.attr("type", type === "password" ? "text" : "password");
        visibility_icon.attr("class", cls === "fas fa-eye" ? "fas fa-eye-slash" : "fas fa-eye")
    });

    pass_gen.click(e => {
        e.preventDefault();
        password.val('');
        // alert(upper_range());
        // return;
        let pass = "";
        // For uppercase
        pass += upper_range();
        pass += upper_range();

        // For lowercase
        pass += lower_range();
        pass += lower_range();

        // For numbers
        pass += num_range();
        pass += num_range();

        // For symbols
        pass += sym_range();
        pass += sym_range();

        pass = pass.split('').sort(function(){return 0.5-Math.random()}).join('');
        password.val(pass);
    });

    const range = (min, max) => {
        return min+Math.floor(Math.random() * (max-min+1));
    }

    const upper_range = () => {
        return String.fromCharCode(range(65, 90));
    }

    const lower_range = () => {
        return String.fromCharCode(range(97, 122));
    }

    const num_range = () => {
        return String.fromCharCode(range(48, 57));
    }

    const sym_range = () => {
        const r = range(1, 4);
        if (r === 1)
            return String.fromCharCode(range(33,47));
        else if (r === 2)
            return String.fromCharCode(range(58, 64));
        else if (r === 3)
            return String.fromCharCode(range(91, 96));
        return String.fromCharCode(range(123, 126));
    }
    // function onToggle (e) {
    //     e.preventDefault();
    //     const type = password.attr('type');
    //     alert("It was clicked"+type);
    // }

});