let map, marker, geocoder;

window.initMap = function () {
    const latInput = document.getElementById("latitud");
    const lonInput = document.getElementById("longitud");

    const lat = parseFloat(latInput?.value) || -34.9045;
    const lng = parseFloat(lonInput?.value) || -56.1850;
    const posicionInicial = { lat: lat, lng: lng };

    map = new google.maps.Map(document.getElementById("mapaEstablecimiento"), {
        center: posicionInicial,
        zoom: 15,
    });

    marker = new google.maps.Marker({
        map: map,
        position: posicionInicial,
        draggable: true
    });

    geocoder = new google.maps.Geocoder();
    actualizarDireccion(posicionInicial);
    setInputsLatLon(posicionInicial.lat, posicionInicial.lng);

    marker.addListener("dragend", () => {
        const pos = marker.getPosition();
        setInputsLatLon(pos.lat(), pos.lng());
        actualizarDireccion(pos);
    });

    map.addListener("click", (e) => {
        marker.setPosition(e.latLng);
        setInputsLatLon(e.latLng.lat(), e.latLng.lng());
        actualizarDireccion(e.latLng);
    });
};

function setInputsLatLon(lat, lon) {
    document.getElementById("latitud").value = lat.toFixed(6).replace(",", ".");
    document.getElementById("longitud").value = lon.toFixed(6).replace(",", ".");
}

function actualizarDireccion(posicion) {
    geocoder.geocode({ location: posicion }, (results, status) => {
        if (status === "OK" && results[0]) {
            const direccionInput = document.getElementById("direccion");
            if (direccionInput) {
                direccionInput.value = results[0].formatted_address;
            }
        }
    });
}
