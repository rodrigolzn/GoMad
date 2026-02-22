// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

// Small helpers for onboarding inputs (numeric keyboards & simple filtering)
document.addEventListener('DOMContentLoaded', function(){
	function onlyDigits(e){
		const allowed = ['Backspace','ArrowLeft','ArrowRight','Delete','Tab'];
		if(allowed.includes(e.key)) return;
		if(!/^[0-9]$/.test(e.key)) e.preventDefault();
	}

	document.querySelectorAll('input[inputmode="tel"], input[inputmode="numeric"]').forEach(function(el){
		el.addEventListener('keypress', onlyDigits);
	});
});

// Extra helpers used on Main page for the big mic toggle
document.addEventListener('DOMContentLoaded', function(){
	const micToggle = document.getElementById('micToggle');
	const continueMain = document.getElementById('continueMain');
	if(!micToggle) return;

	let stream = null;
	let allowed = false;

	function setOn(){ micToggle.classList.remove('off'); micToggle.classList.add('on'); micToggle.textContent = 'MICRÓFONO ENCENDIDO'; micToggle.setAttribute('aria-pressed','true'); continueMain.disabled = false; }
	function setOff(){ micToggle.classList.remove('on'); micToggle.classList.add('off'); micToggle.textContent = 'MICRÓFONO APAGADO'; micToggle.setAttribute('aria-pressed','false'); continueMain.disabled = true; }

	micToggle.addEventListener('click', async function(){
		if(!allowed){
			try{
				stream = await navigator.mediaDevices.getUserMedia({ audio: true });
				allowed = true;
				setOn();
			}catch(e){
				allowed = false;
				setOff();
				alert('No se ha concedido permiso de micrófono. Comprueba los ajustes del navegador.');
			}
		} else {
			if(stream){ stream.getTracks().forEach(t=>t.stop()); stream = null; }
			allowed = false;
			setOff();
		}
	});
});
