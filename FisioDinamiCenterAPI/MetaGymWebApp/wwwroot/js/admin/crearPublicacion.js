function previewMedia() {
    const input = document.querySelector('input[name=\"ArchivosMedia\"]');
    const preview = document.getElementById('preview');
    preview.innerHTML = '';

    if (input.files) {
        [...input.files].forEach(file => {
            if (file.type.startsWith('image/')) {
                const reader = new FileReader();
                reader.onload = e => {
                    const img = document.createElement('img');
                    img.src = e.target.result;
                    img.classList.add('rounded');
                    img.style.width = '120px';
                    img.style.height = '120px';
                    img.style.objectFit = 'cover';
                    preview.appendChild(img);
                };
                reader.readAsDataURL(file);
            } else if (file.type.startsWith('video/')) {
                const icon = document.createElement('div');
                icon.innerHTML = '<i class=\"bi bi-camera-video-fill\"></i>';
                icon.classList.add('d-flex', 'align-items-center', 'justify-content-center', 'bg-secondary', 'rounded');
                icon.style.width = '120px';
                icon.style.height = '120px';
                icon.style.fontSize = '2rem';
                preview.appendChild(icon);
            }
        });
    }
}
function previewMedia() {
    const input = document.querySelector('input[name="ArchivosMedia"]');
    const preview = document.getElementById('preview');
    preview.innerHTML = '';

    if (input.files) {
        [...input.files].forEach(file => {
            if (file.type.startsWith('image/')) {
                const reader = new FileReader();
                reader.onload = e => {
                    const img = document.createElement('img');
                    img.src = e.target.result;
                    img.classList.add('rounded', 'me-2', 'mb-2');
                    img.style.width = '120px';
                    img.style.height = '120px';
                    img.style.objectFit = 'cover';
                    preview.appendChild(img);
                };
                reader.readAsDataURL(file);
            } else if (file.type.startsWith('video/')) {
                const reader = new FileReader();
                reader.onload = e => {
                    const video = document.createElement('video');
                    video.src = e.target.result;
                    video.controls = true;
                    video.classList.add('rounded', 'me-2', 'mb-2');
                    video.style.width = '120px';
                    video.style.height = '120px';
                    video.style.objectFit = 'cover';
                    preview.appendChild(video);
                };
                reader.readAsDataURL(file);
            }
        });
    }
}