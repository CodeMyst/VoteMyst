/*
    time.js - Copyright © YilianSource 2020

    Provides functionality to handle displayed times.

    Elements should be decorated with the --unix-timestamp attribute to specify
      their timestamp.

    Timers can be created using the 'timer' class. Timers will not tick into
      negative timespans.

    UTC dates can be formatted into local time using the 'local-date' class. By
      default, this will format into both date and time. This can be overriden
      with the '--date-only' attribute.
*/



// ~~ Conversion Constants ~~
const msSecond = 1000;
const msMinute = msSecond * 60;
const msHour = msMinute * 60;
const msDay = msHour * 24;

// ~~ Defaults ~~
const dateFormat = "dd/MM/yyyy";
const dateTimeFormat = "dd/MM/yyyy HH:mm:ss";
const timerFormat = "d.HH:mm:ss";

// ~~ Attributes ~~
const unixAttribute = '--unix-timestamp';
const dateOnlyAttribute = '--date-only';

// ~~ Object Classes ~~
const timerClass = '.timer';
const dateClass = '.local-date';



// ~~ Perform initialization ~~
formatDates();
createTimers();



// ~~ Initialization methods ~~

// Formats all dates on the page to the local time.
// The format can be overriden with the '--date-only' attribute.
function formatDates() {
    document.querySelectorAll(dateClass).forEach(dateElement => {
        let date = getDateFromElement(dateElement);
        let format = dateElement.hasAttribute(dateOnlyAttribute) ? dateFormat : dateTimeFormat;
        dateElement.innerHTML = formatDate(date, format);
    });
}

// Creates and initializes all timers on the page.
// Timers will tick down until they reach the specified unix timestamp. 
// The format always includes days, hours, minutes and seconds.
function createTimers() {
    document.querySelectorAll(timerClass).forEach(timer => {
        let targetTime = getDateFromElement(timer).getTime();
        
        // A function that recalculates and displays the distance to the target time.
        function timerTick() {
            let now = new Date().getTime();

            let distance = targetTime - now;
            if (distance < 0) {
                distance = 0;
                clearInterval(tickInterval);
            }

            let formattedTimespan = formatTimespan(distance, timerFormat);
            timer.innerHTML = formattedTimespan;
        }
    
        let tickInterval = setInterval(timerTick, 500);
        timerTick();
    });
}

// ~~ Helper functions ~~

// Returns the set unix timestamp from the given DOM element.
function getUnixFromElement(e) {
    return parseInt(e.getAttribute(unixAttribute));
}

// Creates a JS Date object from the specified unix timestamp.
function getDateFromUnix(u) {
    return new Date(u * 1000);
}

// Retrives the set unix timestamp from the given DOM element and converts it into a JS Date object.
function getDateFromElement(e) {
    return getDateFromUnix(getUnixFromElement(e));
}

// Formats the given integer to include at least two digits (may append leading zeros).
function formatTwoDigits(n) {
    return n.toString().padStart(2, '0');
}

// Formats the date using the given format string.
// Formatting options include dd, MM, yyyy, HH, mm, ss.
function formatDate(date, format) {
    return format
        .replace("dd", formatTwoDigits(date.getDate()))
        .replace("MM", formatTwoDigits(date.getMonth() + 1))
        .replace("yyyy", date.getFullYear())
        .replace("HH", formatTwoDigits(date.getHours()))
        .replace("mm", formatTwoDigits(date.getMinutes()))
        .replace("ss", formatTwoDigits(date.getSeconds()));
}

// Formats the given timespan (in milliseconds) using the given format string.
// Formatting options include d, HH, mm, ss.
function formatTimespan(totalMs, format) {    
    let days = Math.floor(totalMs / msDay);
    let hours = Math.floor(totalMs % msDay / msHour);
    let minutes = Math.floor(totalMs % msHour / msMinute);
    let seconds = Math.floor(totalMs % msMinute / msSecond);

    return format
        .replace("d", days)
        .replace("HH", formatTwoDigits(hours))
        .replace("mm", formatTwoDigits(minutes))
        .replace("ss", formatTwoDigits(seconds));
}