let updateProfileForm =
{
    userid: "",
    email: "",
    username: "",
    phone: "",
    birthdate: "",
    gender: "",
    displayname: "",
    profpicfile: "",
    firstname: "",
    lastname: "",
    middlename:"",
    isTwoFactorOn: "",
    isPhoneVerified: "",
    isEmailVerified: "",
    isTermsAccepted: "",

};

function GetUserByUsername(username) {

    $.ajax({
        type: "GET",
        url: "/api/v1/Profile/GetUserProfile/" + username,
        dataType: "json",
        success: function (result) {
            let middlename = (result.middlename !== null) ? result.middlename.toUpperCase() : "";
            $("#fullName").text(result.firstname.toUpperCase() + " " + middlename + " " + result.lastname.toUpperCase());
            $("#firstname").val(result.firstname);
            $("#middlename").val(result.middlename);
            $("#appUserRole").text(result.userRole);
            $("#lastname").val(result.lastname);
            $("#email").val(result.email);
            $("#username").val(result.username);
            $("#phone").val(result.phone);
            $("#displayname").val(result.displayname);

            let gender = (result.gender == null) ? "Select Gender" : result.gender;
            $("#gender").val(gender).trigger("chosen:updated");

            if (result.birthday == null || result.birthday === "") {
                $("#birthdate").datepicker("setDate", null)
            }
            else {
                $("#birthdate").datepicker("setDate", new Date(result.birthday));
            }

            $("#isEmailVerified").prop('checked', result.isEmailVerified);
            $("#isTwoFactorOn").prop('checked', result.isTwoFactorOn);
            $("#isPhoneVerified").prop('checked', result.isPhoneVerified);
            $("#isTermsAccepted").prop('checked', result.isTermsAccepted);

            let profilePic = (result.profilePic == null) ? "/uploads/user/profile/default/profile.jpeg" : result.profilePic;
            $("#imgProfile").attr('src', profilePic);

        },
        error: function (err) {
            console.log(err)
        }


    function UpdateUser(data)
    {
            const formData = new FormData();
            for(const key of Object.keys(data)) {
        const value = data[key];
        formData.append(key, value);
    }
    Swal.fire({
        title: 'Enter your password',
        input: 'password',
        inputAttributes: {
            autocapitalize: 'off'
        },
        showCancelButton: true,
        confirmButtonText: 'Update Profile',
        showLoaderOnConfirm: true,
        preConfirm: (password) => {
            formData.append("password", password);
            $.ajax({
                type: 'POST',
                url: "/api/v1/Profile/UpdateProfile",
                data: formData,
                contentType: false,
                processData: false,
                headers: {
                    'Accept': 'multipart/form-data',
                    'X-XSRF-TOKEN': getCookie("XSRF-TOKEN")
                },
                success: function (result) {
                    Swal.fire(
                        'Profile Updated',
                        result,
                        'success'
                    ).then(GetUserByUsername(data.username))
                },
                error: function (jqXHR, textStatus, errorThrown) {
                    // Split the error string:
                    const errors = jqXHR.responseText.split(",");

                    Swal.fire({
                        title: 'Error!',
                        text: 'Profile Could not be updated'
                    })
                }
            });
        }
    })
    }   
}
        
    


function UpdateUser(data) {
    const formData = new FormData();

    for (const key of Object.keys(data)) {
        const value = data[key];
        formData.append(key, value);
    }

    Swal.fire({
        title: 'Enter your password',
        input: 'password',
        inputAttributes: {
            autocapitalize: 'off'
        },
        showCancelButton: true,
        confirmButtonText: 'Update Profile',
        showLoaderOnConfirm: true,
        preConfirm: (password) => {
            formData.append("password", password);
            $.ajax({
                type: 'POST',
                url: "/api/v1/Profile/UpdateProfile",
                data: formData,
                contentType: false,
                processData: false,
                headers: {
                    'Accept': 'multipart/form-data',
                    'X-XSRF-TOKEN': getCookie("XSRF-TOKEN")
                },
                success: function (result) {
                    Swal.fire(
                        'Profile Updated',
                        result,
                        'success'
                    ).then(GetUserByUsername(data.username))
                },
                error: function (jqXHR, textStatus, errorThrown) {
                    // Split the error string:
                    const errors = jqXHR.responseText.split(",");

                    Swal.fire({
                        title: 'Error!',
                        text: 'Profile Could not be updated'
                    })
                }
            });
        }
    })
}

function triggerInput() {
    $("#profpicfile").trigger('click');
}

function onFileChanged(event) {
    if (event.files && event.files[0]) {
        let reader = new FileReader();
        let file = event.files[0];
        updateProfileForm.profpicfile = file;
        reader.readAsDataURL(file);
        reader.onload = () => {
            $("#profpic").find('img').attr('src', reader.result);
        };
    }
}


/* EXTENSION METHOD TO GET BROWSER COOKIE */
function getCookie(cname) {
    var name = cname + "=";
    var decodedCookie = decodeURIComponent(document.cookie);
    var ca = decodedCookie.split(';');
    for (var i = 0; i < ca.length; i++) {
        var c = ca[i];
        while (c.charAt(0) === ' ') {
            c = c.substring(1);
        }
        if (c.indexOf(name) === 0) {
            return c.substring(name.length, c.length);
        }
    }
    return "";
}