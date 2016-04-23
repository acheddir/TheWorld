// site.js
(function () {

    //$('#username').text('Abderrahman Cheddir');

    //$('#main').on('mouseenter', function () {
    //    this.style = 'background-color: #888;';
    //});
    //$('#main').on('mouseleave', function () {
    //    this.style = '';
    //});

    //$('ul.menu li a').on('click', function () {
    //    var me = $(this);
    //    alert(me.text());
    //});

    var $sidebarAndWrapper = $('#sidebar,#wrapper');
    var $icon = $('#sidebarToggle i.fa');

    $('#sidebarToggle').on('click', function () {
        $sidebarAndWrapper.toggleClass('hide-sidebar');
        if ($sidebarAndWrapper.hasClass('hide-sidebar')) {
            $icon.removeClass('fa-angle-left');
            $icon.addClass('fa-angle-right');
        } else {
            $icon.addClass('fa-angle-left');
            $icon.removeClass('fa-angle-right');
        }
    });

})();