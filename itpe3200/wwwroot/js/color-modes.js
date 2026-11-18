/*!
 * Color mode toggler, adapted from Bootstrap's docs (https://getbootstrap.com/docs/5.3/customize/color-modes/)
 * Copyright 2011-2026 The Bootstrap Authors
 * Licensed under the Creative Commons Attribution 3.0 Unported License.
 * Simplified for this project: light/dark only, light as default.
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

    // Mark the button for the active theme
    const showActiveTheme = theme => {
        const btnToActive = document.querySelector(`[data-bs-theme-value="${theme}"]`)

        // Stop if page has no theme buttons
        if (!btnToActive) {
            return
        }
        
        // Remove marking from all buttons
        document.querySelectorAll('[data-bs-theme-value]').forEach(element => {
            element.classList.remove('active')
            element.setAttribute('aria-pressed', 'false')
        })

        // Mark the active one
        btnToActive.classList.add('active')
        btnToActive.setAttribute('aria-pressed', 'true')
    }

    // When the page has loaded: mark the active button and listen for clicks on the theme buttons
    window.addEventListener('DOMContentLoaded', () => {
        showActiveTheme(getPreferredTheme())

        document.querySelectorAll('[data-bs-theme-value]')
            .forEach(toggle => {
                toggle.addEventListener('click', () => {
                    const theme = toggle.getAttribute('data-bs-theme-value')
                    setStoredTheme(theme)
                    setTheme(theme)
                    showActiveTheme(theme)
                })
            })
    })
})()