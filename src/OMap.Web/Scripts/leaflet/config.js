
var map;
var marker;

var mapOptions = {
    worldCopyJump: false,
    preferCanvas: true
};


$(function () {

    //var osmUrl = 'http://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png',
    //	osmAttrib = '&copy; <a href="http://openstreetmap.org/copyright">OpenStreetMap</a> contributors',
    //	osm = L.tileLayer(osmUrl, { maxZoom: 18, attribution: osmAttrib });

    var offlineLayer = 'http://localhost:62702/api/tile/{z}/{x}/{y}';
    var copyright = '&copy; <a href="http://openstreetmap.org/copyright">Bader Offline Map</a> contributors';
    var offlineLayer = L.tileLayer(offlineLayer, { minZoom: 2, maxZoom: 4, attribution: copyright });

    var map = L.map('map', mapOptions);//.setView([51.505, -0.159], 15).addLayer(offlineLayer);
    map.addLayer(offlineLayer);
    map.setView([51.505, -0.159], 15);

    marker = L.marker([50.5, 30.5]);
    map.addLayer(marker);
});