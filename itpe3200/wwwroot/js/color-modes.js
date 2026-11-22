/*!
 * Color mode toggler, adapted from Bootstrap's docs (https://getbootstrap.com/docs/5.3/customize/color-modes/)
 * Copyright 2011-2026 The Bootstrap Authors
 * Licensed under the Creative Commons Attribution 3.0 Unported License.
 * Simplified for this project: one button that toggles between light and dark, light as default.
 */

(() => {
    'use strict'

    // Read and save the chosen theme in local storage
    const getStoredTheme = () => localStorage.getItem('theme')
    const setStoredTheme = theme => localStorage.setItem('theme', theme)

    // Use the preferred theme, default = light
    const getPreferredTheme = () => {
        const storedTheme = getStoredTheme()
        if (storedTheme) {
            return storedTheme
        }
        return 'light'
    }

    // Apply the selected theme on page using the data-bs-theme attribute 
    const setTheme = theme => {
        document.documentElement.setAttribute('data-bs-theme', theme)
    }

    // Set theme right away before page is drawn
    setTheme(getPreferredTheme())

    // When the page has loaded: switch theme each time the button is clicked
    window.addEventListener('DOMContentLoaded', () => {
        const toggle = document.getElementById('theme-toggle')

        // Stop if the page has no theme button
        if (!toggle) {
            return
        }

        toggle.addEventListener('click', () => {
            const current = document.documentElement.getAttribute('data-bs-theme')
            const next = current === 'dark' ? 'light' : 'dark'
            setStoredTheme(next)
            setTheme(next)
        })
    })
})()