
function validateSignup() {
    var fullname = document.getElementById("fullname").value;
    var phone = document.getElementById("phone").value;
    var email = document.getElementById("email").value;
    var password = document.getElementById("password").value;
    var message = document.getElementById("signupMessage");

    
    if (fullname.trim() === "") {
        message.innerHTML = "Please enter your full name.";
        message.style.color = "red";
        return false;
    }

    
    if (phone.substring(0, 2) !== "05") {
        message.innerHTML = "Please enter a Saudi phone number starting with 05 and containing 10 digits.";
        message.style.color = "red";
        return false;
    }

    
    if (phone.length !== 10) {
        message.innerHTML = "Phone number must contain exactly 10 digits.";
        message.style.color = "red";
        return false;
    }

    
    for (var i = 0; i < phone.length; i++) {
        if (phone[i] < "0" || phone[i] > "9") {
            message.innerHTML = "Phone number must contain numbers only.";
            message.style.color = "red";
            return false;
        }
    }

    
    if (email.indexOf("@") <= 0 ||
        email.lastIndexOf(".") <= email.indexOf("@") + 1 ||
        email.lastIndexOf(".") === email.length - 1) {

        message.innerHTML = "Please enter a valid email address.";
        message.style.color = "red";
        return false;
    }

    
    if (password.length < 8) {
        message.innerHTML = "Password must be at least 8 characters.";
        message.style.color = "red";
        return false;
    }

    
    message.innerHTML = "Signup form is valid!";
    message.style.color = "green";

    return false;
}


function validateLogin() {
    var email = document.getElementById("loginEmail").value;
    var password = document.getElementById("loginPassword").value;
    var message = document.getElementById("loginMessage");

  
    if (email.trim() === "" || password === "") {
        message.innerHTML = "Please fill in all fields.";
        message.style.color = "red";
        return false;
    }

   
    if (email.indexOf("@") <= 0 ||
        email.lastIndexOf(".") <= email.indexOf("@") + 1 ||
        email.lastIndexOf(".") === email.length - 1) {

        message.innerHTML = "Please enter a valid email address.";
        message.style.color = "red";
        return false;
    }

    
    if (password.length < 8) {
        message.innerHTML = "Password must be at least 8 characters.";
        message.style.color = "red";
        return false;
    }

    
    message.innerHTML = "Login form is valid!";
    message.style.color = "green";

    return false;
}

function showGymPhoto(area) {
    var photo = document.getElementById("gymPhoto");
    var title = document.getElementById("gymTitle");
    var description = document.getElementById("gymDescription");

    if (area === "interior") {
        photo.src = "images/gym-interior.jpeg";
        photo.alt = "Gym Interior";
        title.innerHTML = "Our Gym Interior";
        description.innerHTML = "Take a look inside our gym.";
    }
    else if (area === "cardio") {
        photo.src = "images/gym-cardio.png";
        photo.alt = "Cardio Training Area";
        title.innerHTML = "Cardio Training Area";
        description.innerHTML = "Explore our modern cardio equipment.";
    }
    else if (area === "weights") {
        photo.src = "images/gym-weights.png";
        photo.alt = "Weight Training Area";
        title.innerHTML = "Weight Training Area";
        description.innerHTML = "Discover our strength training equipment.";
    }
}

function showGymBenefits() {
    var text = document.getElementById("gymBenefitText");

    text.innerHTML = "Our gym offers modern equipment, cardio training, strength training, and a comfortable workout environment.";

    text.style.color = "orange";
    text.style.backgroundColor = "#333333";
    text.style.padding = "15px";
}
