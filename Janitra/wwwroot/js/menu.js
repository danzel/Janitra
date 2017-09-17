window.document.getElementsByClassName('navbar-burger')[0].addEventListener('click',
    function() {
        var menu = window.document.getElementsByClassName('navbar-menu')[0];
        if (menu.classList.contains('is-active'))
            menu.classList.remove('is-active');
        else
            menu.classList.add('is-active');
    }
);