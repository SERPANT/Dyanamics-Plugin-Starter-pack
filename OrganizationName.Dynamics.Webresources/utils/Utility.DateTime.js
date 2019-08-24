/**
 * Return difference between date and today.
 *
 * @param {string} date : Date String.
 * @returns {Date} : date difference.
 */
function diffTillToday(date) {
    const clientBirthDate = new Date(date);

    return Date.now() - clientBirthDate;
}

/**
 * Converts millisecond to years.
 *
 * @param {int} millisecond : millisecond.
 * @returns {int} : years
 */
function milliSecondToYear(millisecond) {
    const milliSecondToYear = 60 * 60 * 24 * 365 * 1000;

    return parseInt(millisecond / milliSecondToYear);
}

