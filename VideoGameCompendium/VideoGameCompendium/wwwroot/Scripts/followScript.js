console.log("boop");

document.addEventListener("DOMContentLoaded", function (e) {
	console.log("boop");

	var buttons = document.getElementsByClassName("followButton");
	console.log(buttons);
	for (var i = 0; i < buttons.length; i++) {

		buttons[i].onclick = function (e) {

			var loggedInId;
			var leaderId;

			if (e.target.classList.contains("followButton")) {
				loggedInId = e.target.getElementsByClassName("loggedInId")[0].innerHTML;
				leaderId = e.target.getElementsByClassName("leaderId")[0].innerHTML;
			}
			else {
				loggedInId = e.target.parentElement.getElementsByClassName("loggedInId")[0].innerHTML;
				leaderId = e.target.parentElement.getElementsByClassName("leaderId")[0].innerHTML;
			}

			var xmlhttp = new XMLHttpRequest();
			xmlhttp.onreadystatechange = function () {
				if (xmlhttp.readyState === 4) {
					console.log("Server response " + xmlhttp.response);
					if (xmlhttp.response !== "Error")
					{
						var p;
						if (e.target.classList.contains("followButton")) 
							p = e.target.getElementsByTagName("p")[0];
						else 
							p = e.target;

						console.log(p);
						if (xmlhttp.response == 1)
							p.innerHTML = "Unfollow";
						else
							p.innerHTML = "Follow!";
					}
				}
			}
			xmlhttp.open("GET", "http://192.168.43.160:53227/Home/DoFollow?loggedInId=" + loggedInId + "&leaderId=" + leaderId, "true"); // configure object (method, URL, async)
			xmlhttp.send();
		};
	}
});