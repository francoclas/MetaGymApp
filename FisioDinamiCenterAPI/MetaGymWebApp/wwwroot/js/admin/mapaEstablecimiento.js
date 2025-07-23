let map, marker, geocoder;

//llamo despues de cargar el mapa 
function initMap() {
    const defaultPos = { lat: -34.9045, lng: -56.1850 }; // Centro Montevideo

    // Inicio el mapa
    map = new google.maps.Map(document.getElementById("map"), {
        center: defaultPos,
        zoom: 15,
    });

    // genro el marcador
    marker = new google.maps.Marker({
        map: map,
        position: defaultPos,
        draggable: true
    });

    // Inicializar el geocoder (convierte coordenadas en dirección)
    geocoder = new google.maps.Geocoder();

    // Cargar dirección inicial
    actualizarDireccion(defaultPos);

    marker.addListener("dragend", () => {
        const pos = marker.getPosition();
        document.getElementById("latitud").value = pos.lat();
        document.getElementById("longitud").value = pos.lng();
        actualizarDireccion(pos);
    });

    // cargar el clic en el mapa
    map.addListener("click", (e) => {
        marker.setPosition(e.latLng);
        document.getElementById("latitud").value = e.latLng.lat();
        document.getElementById("longitud").value = e.latLng.lng();
        actualizarDireccion(e.latLng);
    });
}

//convertir las coordenadas en direccion
function actualizarDireccion(posicion) {
    geocoder.geocode({ location: posicion }, (results, status) => {
        if (status === "OK" && results[0]) {
            document.getElementById("inputDireccion").value = results[0].formatted_address;
        } else {
            document.getElementById("inputDireccion").value = "Dirección no encontrada";
        }
    });
}
