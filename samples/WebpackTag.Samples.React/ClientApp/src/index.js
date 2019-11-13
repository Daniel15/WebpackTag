import 'bootstrap/dist/css/bootstrap.css';
import React from 'react';
import ReactDOM from 'react-dom';
import { BrowserRouter } from 'react-router-dom';
import App from './App';

const rootElement = document.getElementById('root');

ReactDOM.render(
  <BrowserRouter basename="/">
    <App />
  </BrowserRouter>,
  rootElement);
