require('./first.css');

// Just a dummy example of pulling in third party code, so Webpack bundles it.
const React = require('react');
const ReactDOM = require('react-dom');
window._test = ReactDOM;

document.getElementById('hello').innerHTML = 'Hello from first.js';