// site.js

/**
 * Kích hoạt animation fade-in cho caption của carousel khi slide.
 */
function setupCarouselAnimation() {
    $('.carousel').on('slide.bs.carousel', function (e) {
        $('.carousel-caption').hide().fadeIn(1000);
    });
}

/**
 * Kích hoạt animation hover cho các phần tử có class .card và .btn.
 */
function setupHoverAnimation() {
    $('.card, .btn').hover(
        function () {
            $(this).css('transform', 'scale(1.05)').css('box-shadow', '0 0 10px rgba(0,0,0,0.2)');
        },
        function () {
            $(this).css('transform', 'scale(1)').css('box-shadow', 'none');
        }
    );
}

/**
 * Định dạng tự động cho trường Card Number (XXXX-XXXX-XXXX-XXXX) khi người dùng nhập.
 */
function formatCardNumberInput() {
    $(document).on("input", "#cardNumberInput", function (e) {
        console.log("Binding auto-format to #cardNumberInput with event delegation");

        // Lấy giá trị hiện tại và loại bỏ tất cả ký tự không phải số
        let value = $(this).val().replace(/\D/g, "");

        // Giới hạn 16 chữ số
        if (value.length > 16) {
            value = value.substring(0, 16);
            e.preventDefault(); // Ngăn nhập thêm ký tự
        }

        // Định dạng thành XXXX-XXXX-XXXX-XXXX
        let formattedValue = "";
        for (let i = 0; i < value.length; i++) {
            if (i > 0 && i % 4 === 0) {
                formattedValue += "-";
            }
            formattedValue += value[i];
        }

        // Cập nhật giá trị vào input
        $(this).val(formattedValue);

        // Ngăn nhập thêm nếu đã đủ 16 chữ số (19 ký tự với dấu gạch ngang)
        if (value.length >= 16) {
            $(this).prop("maxlength", 19); // Giới hạn độ dài tối đa
            e.preventDefault();
        } else {
            $(this).prop("maxlength", 19); // Đảm bảo maxlength luôn là 19
        }
    });
}

/**
 * Định dạng tự động cho trường Card Number (XXXX-XXXX-XXXX-XXXX) khi người dùng nhập.
 */
function formatCardNumberInput() {
    $("#cardNumberInput").on("input", function (e) {
        console.log("Binding auto-format to #cardNumberInput");

        // Lấy giá trị hiện tại và loại bỏ tất cả ký tự không phải số
        let value = $(this).val().replace(/\D/g, "");

        // Giới hạn 16 chữ số
        if (value.length > 16) {
            value = value.substring(0, 16);
            e.preventDefault();
        }

        // Định dạng thành XXXX-XXXX-XXXX-XXXX
        let formattedValue = "";
        for (let i = 0; i < value.length; i++) {
            if (i > 0 && i % 4 === 0) {
                formattedValue += "-";
            }
            formattedValue += value[i];
        }

        // Cập nhật giá trị vào input
        $(this).val(formattedValue);

        // Ngăn nhập thêm nếu đã đủ 16 chữ số
        if (value.length >= 16) {
            $(this).prop("maxlength", 19);
            e.preventDefault();
        } else {
            $(this).prop("maxlength", 19);
        }
    });
}

/**
 * Định dạng tự động cho trường Expiry Date (MM-YY) khi người dùng nhập.
 */
function formatExpiryDateInput() {
    $("#expiryDateInput").on("input", function (e) {
        console.log("Binding auto-format to #expiryDateInput");

        // Lấy giá trị hiện tại và loại bỏ tất cả ký tự không phải số
        let value = $(this).val().replace(/\D/g, "");

        // Giới hạn 4 chữ số
        if (value.length > 4) {
            value = value.substring(0, 4);
            e.preventDefault();
        }

        // Định dạng thành MM-YY
        let formattedValue = "";
        if (value.length > 0) {
            let month = value.substring(0, 2);
            formattedValue = month.length === 1 ? "0" + month : month;
        }
        if (value.length > 2) {
            formattedValue += "-";
            let year = value.substring(2, 4);
            formattedValue += year;
        }

        // Cập nhật giá trị vào input
        $(this).val(formattedValue);

        // Ngăn nhập thêm nếu đã đủ 4 chữ số
        if (value.length >= 4) {
            $(this).prop("maxlength", 5);
            e.preventDefault();
        } else {
            $(this).prop("maxlength", 5);
        }
    });
}
/**
 * Định nghĩa phương thức tùy chỉnh để kiểm tra xem có radio button nào được chọn không.
 */
function addRequireOneValidationMethod() {
    $.validator.addMethod("requireOne", function (value, element) {
        return $('input[type="radio"][name="' + element.name + '"]:checked').length > 0;
    }, "Card Type is required.");
}

/**
 * Định nghĩa phương thức tùy chỉnh để kiểm tra tính hợp lệ của PhonePrefix dựa trên CountryID.
 */
function addPrefixMatchesCountryValidationMethod() {
    $.validator.addMethod("prefixMatchesCountry", function (value, element) {
        var countryID = $("#countryID").val();
        var phonePrefix = value;

        // Mapping of CountryID to PhonePrefix
        var countryPrefixMap = {
            "1": "+1",    // USA
            "2": "+84",   // Vietnam
            "3": "+44",   // UK
            "4": "+1",    // Canada
            "5": "+61",   // Australia
            "6": "+44",   // England (same as UK)
            "7": "+81",   // Japan
            "8": "+86",   // China
            "9": "+65",   // Singapore
            "10": "+856"  // Lao
        };

        // If either field is empty, let other validations handle it
        if (!countryID || !phonePrefix) {
            return true;
        }

        // Check if the selected prefix matches the expected prefix for the selected country
        var expectedPrefix = countryPrefixMap[countryID];
        return phonePrefix === expectedPrefix;
    }, "Phone prefix must match the selected country.");
}

/**
 * Định nghĩa phương thức tùy chỉnh để kiểm tra ngày hết hạn (Expiry Date) có phải là ngày trong tương lai không.
 */
function addFutureDateValidationMethod() {
    $.validator.addMethod("futureDate", function (value, element) {
        if (!value) return true; // Let required validation handle empty values

        // Parse MM-YY format (e.g., 01-25)
        var parts = value.split("-");
        if (parts.length !== 2) return false;

        var month = parseInt(parts[0], 10); // e.g., 01
        var year = parseInt(parts[1], 10);  // e.g., 25

        // Convert year to full year (e.g., 25 -> 2025)
        var fullYear = 2000 + year; // Assuming 21st century (25 -> 2025)

        // Get current date
        var currentDate = new Date();
        var currentMonth = currentDate.getMonth() + 1; // Months are 0-based in JS
        var currentYear = currentDate.getFullYear();

        // Compare dates (future date if year > currentYear OR same year but month >= currentMonth)
        if (fullYear > currentYear) {
            return true;
        } else if (fullYear === currentYear) {
            return month >= currentMonth;
        }
        return false;
    }, "Expiry Date must be a future date.");
}

/**
 * Định nghĩa phương thức tùy chỉnh để kiểm tra loại file (DocumentUpload).
 */
function addFileTypeValidationMethod() {
    $.validator.addMethod("filetype", function (value, element, param) {
        if (!value) return true; // Let required validation handle empty values

        var fileExtension = value.substring(value.lastIndexOf('.')) || "";
        var pattern = new RegExp(param, "i");
        return pattern.test(fileExtension);
    }, function (params, element) {
        return $(element).data("val-filetype") || "Invalid file type.";
    });
}

/**
 * Định nghĩa phương thức tùy chỉnh để kiểm tra kích thước file (DocumentUpload).
 */
function addFileSizeValidationMethod() {
    $.validator.addMethod("filesize", function (value, element, param) {
        if (!element.files || element.files.length === 0) return true; // Let required validation handle empty values

        var fileSize = element.files[0].size; // Size in bytes
        var maxSize = parseInt(param, 10); // Max size in bytes
        return fileSize <= maxSize;
    }, function (params, element) {
        return $(element).data("val-filesize") || "File size too large.";
    });
}

/**
 * Thiết lập validation cho form PersonalDetails.
 */
function setupPersonalDetailsValidation() {
    $("#PersonalDetails").validate({
        rules: {
            FullName: {
                required: true,
                maxlength: 100
            },
            Email: {
                required: true,
                email: true,
                maxlength: 100
            },
            PhonePrefix: {
                required: true,
                prefixMatchesCountry: true
            },
            PhoneNumber: {
                required: true,
                pattern: /^\d{6,12}$/
            //    phoneNumberFormat: true
            },
            CountryID: {
                required: true
            },
            CardTypeID: {
                requireOne: true
            },
            TermsAgreed: {
                required: true
            }
        },
        errorPlacement: function (error, element) {
            if (element.attr("name") === "PhonePrefix") {
                error.appendTo("#phonePrefixError");
                $("#phonePrefixError").removeClass("d-none");
            } else {
                error.insertAfter(element);
            }
        },
        invalidHandler: function (event, validator) {
            console.log("PersonalDetails form validation failed: ", validator.numberOfInvalids(), " invalid fields");
        },
        submitHandler: function (form) {
            console.log("PersonalDetails form validation passed, submitting form");
            form.submit();
        }
    });

    // Reset error message visibility when prefix or country changes
    $(document).on("change", "#phonePrefix, #countryID", function () {
        $("#phonePrefixError").addClass("d-none");
    });
}

/**
 * Thiết lập validation cho form CreditCardDetails.
 */
function setupCreditCardDetailsValidation() {
    $("#CreditCardDetails").validate({
        rules: {
            ExpiryDate: {
                futureDate: true
            },
            DocumentUpload: {
                filetype: true,
                filesize: true,
                required: true,
                accept: "pdf|jpg|png"
            }
        },
        messages: {
            DocumentUpload: {
                required: "Identification document is required.",
                accept: "Only PDF, JPG, or PNG files are allowed."
            }
        },
        invalidHandler: function (event, validator) {
            console.log("CreditCardDetails form validation failed: ", validator.numberOfInvalids(), " invalid fields");
        },
        submitHandler: function (form) {
            console.log("CreditCardDetails form validation passed, submitting form");
            form.submit();
        }
    });
}

$(document).ready(function () {
    console.log("site.js loaded and running");

    // Thiết lập UI animations
    setupCarouselAnimation();
    setupHoverAnimation();

    // Thiết lập auto-formatting cho CreditCardDetails
    formatCardNumberInput();
    formatExpiryDateInput();
    handleCreditCardDetailsSubmit();

    // Định nghĩa các phương thức validation tùy chỉnh
    addRequireOneValidationMethod();
    addPrefixMatchesCountryValidationMethod();
    addFutureDateValidationMethod();
    addFileTypeValidationMethod();
    addFileSizeValidationMethod();

    // Thiết lập validation cho các form
    setupPersonalDetailsValidation();
    setupCreditCardDetailsValidation();
});