/*Déclarer les variables*/
var map = null;
var lat = null;
var lng = null;
var montreal = {lat: 45.5017, lng: -73.5673};
var markers = [];
var contents = [];
var previousMaker;
var eventDate, date, time;
var eventList = [];
var json = [];
var hour = 12.25;
var time_min;
var time_max;

/*Initialisation de la carte de google map et création du marker*/
function initMap() {

	map = new google.maps.Map(document.getElementById('map'), {
		center: montreal,
		zoom: 12
	});

	//Added them again
	for (var i in json) {
		var marker = new google.maps.Marker ({
				position: new google.maps.LatLng(parseFloat(json[i].location.latitude), parseFloat(json[i].location.longitude)),
				title: json[i].id,
				map: map
	});

		var contentText = '<div class="container-fluid"> <div class= "ui-widget col-sm-8"> <h4>'+ json[i].name +' </h4>'
		+ '<p> Number of participants: ' + json[i].expected + '</p>' + '<p> End Time: ' + json[i].end + '</p> </div>' 
		+ '<div= "ui-widget col-sm-4"><img src =' + json[i].logo + ' style="width:120px;height:120px;" "align:middle" > </img> </div> </div>'
		+ '<div> <b> Description : </b> </br> <textarea "align:middle" cols = "60" rows = "10">' + json[i].description +'</textarea> </div>';

		contents.push(contentText);

		var infoWindow = new google.maps.InfoWindow({
	 		content: contents[i],
	 		maxWidth: 400,
	 		marker: marker
		});  


		google.maps.event.addListener(marker, 'click', (function (marker, i, infoWindow) {
            return function () {
                infoWindow.setContent(contents[i]);
                infoWindow.open(map, this);
        };
        })(marker, i, infoWindow));

		markers.push(marker);
	}


    google.maps.event.addListener(marker, 'click', function() {
  infoWindow.open(map, marker);
});

}

function initAll() {
        /*Getting the list of events and people participating*/
    $.ajax({
            url: "http://localhost:8000/",
            dataType: "json",
            success: function(data) {
                json = data;
                initMap();
                updateStuff();
            }
        });
}


/*Add un marker marker à l'endroit choisi*/

function updateStuff(){
    var d = document.getElementById("date").value.split('-')
    var h = hour;
    var _min = new Date(d[0], d[1]-1, d[2], h, 0, 0)
    var _max = new Date(d[0], d[1]-1, d[2], h, 30, 0)
    time_min = _min.getTime();
    time_max = _max.getTime();
    
    eventList = [];
    for (var i = 0; i < json.length; i++) {
        var endDate = new Date(json[i]["end"]);
        endDate = endDate.getTime();
        if (endDate >= time_min && endDate <= time_max){
            eventList.push(json[i]);
        }
        
    }

    updateMap(eventList);
}

/*Fonctions à exécuter lorsque la page est chargée*/
$(document).ready(function(){


	/*Date Selector Function*/
	var inputdate = document.getElementById("date").value;
	console.log("Date: " + inputdate);

	/*Time Slider Function*/
	$(function() {
		$( "#timeSlider" ).slider({
			min: 0,
			max: 24,
			value : 12,
			step : 0.5,
			slide: function(e, ui){
				if (ui.value > 23.5)
					return false;

				if (ui.value < 11.5) {
					if (Number.isInteger(ui.value))
						$( "#sliderText" ).val(ui.value + ":00 am - " + ui.value + ":30 am");
					else
						$( "#sliderText" ).val((ui.value - 0.5) + ":30 am - " + (ui.value + 0.5) + ":00 am");
				}
				else if (ui.value == 11.5)
					$( "#sliderText" ).val((ui.value - 0.5) + ":30 am - " + (ui.value + 0.5) + ":00 pm");
				else if (ui.value == 12)
					$( "#sliderText" ).val(ui.value + ":00 pm - " + (ui.value) + ":30 pm");
				else if (ui.value == 12.5)
					$( "#sliderText" ).val((ui.value - 0.5) + ":00 pm - " + (ui.value + 0.5 -12) + ":00 pm");
				else if (ui.value == 23.5)
					$( "#sliderText" ).val((ui.value -12 - 0.5) + ":30 pm - " + (ui.value -12 + 0.5) + ":00 am");
				else {
					if (Number.isInteger(ui.value))
						$( "#sliderText" ).val(ui.value -12 + ":00 pm - " + (ui.value -12) + ":30 pm");
					else
						$( "#sliderText" ).val((ui.value -12 - 0.5) + ":30 pm - " + (ui.value -12 + 0.5) + ":00 pm");
				}

				//Appeler la fonction pour changer la map
				 var time = "";
				 if (Number.isInteger(ui.value)) {
					time = ui.value - 0.5 + ":30:00";
				} else {
					time = ui.value + ": 00";
				}
				if (ui.value < 10)
					time = "0" + time;
                hour = ui.value + 0.25;
                updateStuff()

			}
		});
		$( "#sliderText" ).val(12 + ":00 pm - " + 12 + ":30 pm" ); 
        updateStuff();
        });
});

    /*Add the event places to the map*/
    function updateMap(eventList) {
        for (var m = 0; m < markers.length; m++) {
            markers[m].setVisible(false);
        }
        for (var i = 0; i < eventList.length; i++){
            for (var m = 0; m < markers.length; m++) {
                if (markers[m].title == eventList[i].id)
                {
                    markers[m].setVisible(true);
                }
            }
        }
    }

