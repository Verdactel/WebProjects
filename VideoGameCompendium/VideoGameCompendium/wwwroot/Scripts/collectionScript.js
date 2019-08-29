
document.addEventListener("DOMContentLoaded", function (e) {

	var buttons = document.getElementsByClassName("collectionButton");

	for (var i = 0; i < buttons.length; i++) {

		buttons[i].onclick = function (e) {

			var gameId;
			var userId;

			if (e.target.classList.contains("collectionButton")) {
				gameId = e.target.getElementsByClassName("gameId")[0].innerHTML;
				userId = e.target.getElementsByClassName("userId")[0].innerHTML;
			}
			else {
				gameId = e.target.parentElement.getElementsByClassName("gameId")[0].innerHTML;
				userId = e.target.parentElement.getElementsByClassName("userId")[0].innerHTML;
			}

			var xmlhttp = new XMLHttpRequest();
			xmlhttp.onreadystatechange = function () {
				if (xmlhttp.readyState === 4) {
					console.log("Server response " + xmlhttp.response);
					if (xmlhttp.response !== "Error")
					{
						var p;
						if (e.target.classList.contains("collectionButton")) 
							p = e.target.getElementsByTagName("p")[0];
						else 
							p = e.target;

						console.log(p);
						if (xmlhttp.response == 1)
							p.innerHTML = "Remove from Collection";
						else
							p.innerHTML = "Add to Collection";
					}
				}
			}
			xmlhttp.open("GET", "http://192.168.43.160:53227/Home/DoCollection?gameId=" + gameId + "&userId=" + userId, "true"); // configure object (method, URL, async)
			xmlhttp.send();
		};
	}
});