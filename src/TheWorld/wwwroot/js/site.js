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

    // var $sidebarAndWrapper = $('#sidebar,#wrapper');
    // var $icon = $('#sidebarToggle i.fa');

    // $('#sidebarToggle').on('click', function () {
    //     $sidebarAndWrapper.toggleClass('hide-sidebar');
    //     if ($sidebarAndWrapper.hasClass('hide-sidebar')) {
    //         $icon.removeClass('fa-angle-left');
    //         $icon.addClass('fa-angle-right');
    //     } else {
    //         $icon.addClass('fa-angle-left');
    //         $icon.removeClass('fa-angle-right');
    //     }
    // });
    
    function htmlbodyHeightUpdate(){
		var height3 = $( window ).height()
		var height1 = $('.nav').height()+50
		height2 = $('.main').height()
		if(height2 > height3){
			$('html').height(Math.max(height1,height3,height2)+10);
			$('body').height(Math.max(height1,height3,height2)+10);
		}
		else
		{
			$('html').height(Math.max(height1,height3,height2));
			$('body').height(Math.max(height1,height3,height2));
		}
	}
    
	$(document).ready(function () {
		htmlbodyHeightUpdate()
		$( window ).resize(function() {
			htmlbodyHeightUpdate()
		});
		$( window ).scroll(function() {
			height2 = $('.main').height()
  			htmlbodyHeightUpdate()
		});
	});

})();