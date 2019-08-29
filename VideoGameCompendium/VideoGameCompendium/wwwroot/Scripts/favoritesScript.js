
document.addEventListener("DOMContentLoaded", function (e) {

    var buttons = document.getElementsByClassName("favoritesButton");

    for (var i = 0; i < buttons.length; i++) {

        buttons[i].onclick = function (e) {

            var gameId;
            var userId;

            if (e.target.classList.contains("favoritesButton")) {
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
                    if (xmlhttp.response !== "Error") {
                        var p;
                        if (e.target.classList.contains("favoritesButton"))
                            p = e.target.getElementsByTagName("p")[0];
                        else
                            p = e.target;

                        console.log(p);
                        if (xmlhttp.response == 1)
                            p.innerHTML = "Remove from Favorites";
                        else
                            p.innerHTML = "Add to Favorites";
                    }
                }
            }
			xmlhttp.open("GET", "http://192.168.43.160:53227/Home/DoFavorites?gameId=" + gameId + "&userId=" + userId, "true"); // configure object (method, URL, async)
            xmlhttp.send();
        };
    }
});