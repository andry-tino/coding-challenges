/**
 * Creates a random sequence of integer numbers.
 * @param {number} length The number of elements in the array.
 * @returns {number[]} A numeric array.
 */
export function createRandomSequence(length) {
    const max = 100;
    return Array.apply(null, Array(length)).map(function() {
        return Math.round(Math.random() * max);
    });
}

/**
 * Converts a numeric array into a string representation (comma separated values).
 * @param {number[]} array The array to convert.
 * @returns {string} A string.
 */
export function array2str(array) {
    return JSON.stringify(array)
        .replaceAll("[", "")
        .replaceAll("]", "");
}