const msSecond = 1000;
const msMinute = msSecond * 60;
const msHour = msMinute * 60;
const msDay = msHour * 24;

function formatFragment(number, minPlaces) {
    return ("0".repeat(minPlaces - 1) + number).slice(-minPlaces);
}

document.querySelectorAll(".timer").forEach(timer => {
    let unixTimestamp = parseInt(timer.getAttribute("--timer-end-unix"));
    let targetTime = new Date(unixTimestamp * 1000).getTime();
    
    function timerTick() {
        let now = new Date().getTime();
        let distance = targetTime - now;

        let days = Math.floor(distance / msDay);
        let hours = Math.floor(distance % msDay / msHour);
        let minutes = Math.floor(distance % msHour / msMinute);
        let seconds = Math.floor(distance % msMinute / msSecond);

        let formattedTime = "0:00:00:00";
        if (distance > 0) {
            formattedTime = days + ":"
                + formatFragment(hours, 2) + ":" 
                + formatFragment(minutes, 2) + ":" 
                + formatFragment(seconds, 2);
        }
        else {
            clearInterval(tickInterval);
        }
        timer.innerHTML = formattedTime;
    }

    let tickInterval = setInterval(timerTick, 500);
    timerTick();
});