﻿
document.addEventListener("DOMContentLoaded", function (e) {

	var ratings = document.getElementsByClassName("stars-outer");

	for (var i = 0; i < ratings.length; i++) {

		var gameId = ratings[i].getElementsByClassName("stars-gameId")[0].innerHTML;
		var userId = ratings[i].getElementsByClassName("stars-userId")[0].innerHTML;

		var f = ratings[i].getElementsByClassName("stars-hidden")[0].innerHTML;
		console.log("Loaded Rating of "+f);
		ratings[i].getElementsByClassName("stars-inner")[0].style.width = (f * 20) + "%";

		if (userId != "") {
			ratings[i].onmouseenter = function (e) {
				e.target.getElementsByClassName("stars-cover")[0].style.visibility = "visible";
			}

			ratings[i].onmouseleave = function (e) {
				e.target.getElementsByClassName("stars-cover")[0].style.visibility = "hidden";
			}

			ratings[i].onmousemove = function (e) {
				var rect;
				if (e.target.classList.contains("stars-outer"))
					rect = e.target.getBoundingClientRect();
				else
					rect = e.target.parentElement.getBoundingClientRect();

				var x = e.clientX - rect.left; 

				var percent = Math.floor(Math.floor(x / rect.width * 5) + 1) * 20;

				if (e.target.classList.contains("stars-outer"))
					e.target.getElementsByClassName("stars-cover")[0].style.width = percent + "%";
				else
					e.target.parentElement.getElementsByClassName("stars-cover")[0].style.width = percent + "%";
			}

			ratings[i].onclick = function (e) {
				var stars = 0;
				if (e.target.classList.contains("stars-outer"))
					stars = e.target.getElementsByClassName("stars-cover")[0].style.width;
				else
					stars = e.target.parentElement.getElementsByClassName("stars-cover")[0].style.width;

				stars = stars.substring(0, stars.length - 1) / 20;

				console.log("Rated " + stars + " sending to server");

				if (userId != "" && gameId != "") {
					var xmlhttp = new XMLHttpRequest();
					xmlhttp.onreadystatechange = function () {
						if (xmlhttp.readyState === 4) {
							console.log("Server response " + xmlhttp.response);
							if (xmlhttp.response != 0) {
								if (e.target.classList.contains("stars-outer"))
									e.target.getElementsByClassName("stars-inner")[0].style.width = (xmlhttp.response * 20) + "%";
								else
									e.target.parentElement.getElementsByClassName("stars-inner")[0].style.width = (xmlhttp.response * 20) + "%";
							}
						}
					}
					xmlhttp.open("GET", "http://192.168.43.160:53227/Home/Rate?stars=" + stars + "&gameId=" + gameId + "&userId=" + userId, "true"); // configure object (method, URL, async)
					xmlhttp.send();
				}
			}
		}
	}
});