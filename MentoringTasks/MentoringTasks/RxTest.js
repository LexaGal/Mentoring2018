document.addEventListener("DOMContentLoaded", function () {
    Rx.Observable.fromEvent(document.querySelector('#input'), 'input')
        .debounceTime(1000)
        .map(event => event.target.value)
        .subscribe(value => document.querySelector('#history').appendChild(document.createElement('li')).append(value));
});