import { AuthenticationGuard } from './_guards/authentication.guard';
import { ListsComponent } from './lists/lists.component';
import { MessagesComponent } from './messages/messages.component';
import { HomeComponent } from './home/home.component';
import { Routes } from '@angular/router';
import { MemberListComponent } from './member-list/member-list.component';

export const appRoutes: Routes = [
    { path: '' , component: HomeComponent },
    {
        path: '',
        runGuardsAndResolvers: 'always',
        canActivate: [AuthenticationGuard],
        children: [
            { path: 'members' , component: MemberListComponent, },
            { path: 'messages' , component: MessagesComponent },
            { path: 'lists' , component: ListsComponent },
        ]
    },
    { path: '**', redirectTo: 'home', pathMatch: 'full' }
];

