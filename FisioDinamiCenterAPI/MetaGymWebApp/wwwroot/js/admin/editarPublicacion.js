    function eliminarMedia(mediaId, publicacionId) {
        if (!confirm("¿Estás seguro de que querés eliminar este archivo?")) return;

    const form = document.createElement('form');
    form.method = 'POST';
    form.action = '/Admin/EliminarMedia';

    const input1 = document.createElement('input');
    input1.type = 'hidden';
    input1.name = 'mediaId';
    input1.value = mediaId;

    const input2 = document.createElement('input');
    input2.type = 'hidden';
    input2.name = 'publicacionId';
    input2.value = publicacionId;

    form.appendChild(input1);
    form.appendChild(input2);
    document.body.appendChild(form);
    form.submit();
    }
