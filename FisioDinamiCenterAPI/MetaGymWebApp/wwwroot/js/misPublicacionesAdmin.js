document.addEventListener("DOMContentLoaded", function () {
    const tabs = document.querySelectorAll("#tabsPublicaciones .nav-link");
    const contents = document.querySelectorAll(".tab-content-publicaciones");

    tabs.forEach(tab => {
        tab.addEventListener("click", () => {
            tabs.forEach(t => t.classList.remove("active"));
            contents.forEach(c => c.classList.add("d-none"));

            tab.classList.add("active");
            const target = tab.dataset.tab;
            document.getElementById(target).classList.remove("d-none");
        });
    });
});
