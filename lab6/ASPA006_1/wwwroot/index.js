document.addEventListener('DOMContentLoaded', (event) => {
    const detailsElement = document.createElement('div');
    detailsElement.className = 'details';
    document.body.appendChild(detailsElement); 
    GetCelebrities();
});

async function GetCelebrities() {
    const response = await fetch('/api/celebrities');
    const celebrities = await response.json();
    const gallery = document.querySelector('.gallery');
    console.log(celebrities);

    for (const celeb of celebrities) {
        const imageElement = document.createElement('img');
        const resp = await fetch(`/api/celebrities/photo/${celeb.reqPhotoPath}`);

        if (resp.ok) {
            const imageBlob = await resp.blob();
            const imageUrl = URL.createObjectURL(imageBlob);

            imageElement.src = imageUrl;
            imageElement.dataset.id = celeb.id;
            imageElement.addEventListener('click', () => {
                ImageClick(celeb.id);
            });
            gallery.appendChild(imageElement);
        } else {
            console.error('Ошибка при загрузке изображения:', resp.statusText);
        }
    }
}

async function ImageClick(id) {
    const response = await fetch(`/api/Lifeevents/Celebrities/${id}`);
    const details = await response.json();
    let detailsElement = document.querySelector('.details');
    detailsElement.innerHTML = '';
    const celebResponse = await fetch(`/api/Celebrities/${id}`)
    const celebName = await celebResponse.json();
    console.log(celebName)
    details.forEach(event => {
      
        const eventElement = document.createElement('p'); // Создаем новый элемент для каждого события
        eventElement.textContent = `${celebName.fullName}         ${event.date}: ${event.description}`; // Замените на нужные поля
        detailsElement.appendChild(eventElement);
    });

    if (details.length === 0) {
        const noEventsElement = document.createElement('p');
        noEventsElement.textContent = 'Нет событий для отображения.';
        detailsElement.appendChild(noEventsElement);
    }
}