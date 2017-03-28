import {
    animate,
    AnimationEntryMetadata,
    state,
    style,
    transition,
    trigger
} from '@angular/core';

const passiveStyle = style({
    opacity: 1,
    transform: 'translateX(0)'
});

const enterStyle = style({
    opacity: 0,
    transform: 'translateX(-100%)'
});

const leaveStyle = style({
    opacity: 0,
    transform: 'translateX(100%)'
});

export const slideInDownAnimation: AnimationEntryMetadata = trigger('routeAnimation', [
    state('*', passiveStyle),
    transition(':enter', [enterStyle, animate('0.8s cubic-bezier(0.55, 0, 0.55, 0.2)')]),
    transition(':leave', [animate('0.8s cubic-bezier(0.25, 0.8, 0.25, 1)', leaveStyle)])
]);
