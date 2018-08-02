import { MessagesResolver } from './_resolvers/messages.resolver';
import { ListsResolver } from './_resolvers/lists.resolver';
import { PreventUnsavedChanges } from './_guards/prevent-unsaved-changes.guard';
import { MemberEditResolver } from './_resolvers/member-edit.resolver';
import { MemberEditComponent } from './members/member-edit/member-edit.component';
import { MemberListResolver } from './_resolvers/member-list.resolver';
import { AuthenticationGuard } from './_guards/authentication.guard';
import { ListsComponent } from './lists/lists.component';
import { MessagesComponent } from './messages/messages.component';
import { HomeComponent } from './home/home.component';
import { Routes } from '@angular/router';
import { MemberListComponent } from './members/member-list/member-list.component';
import { MemberDetailComponent } from './members/member-detail/member-detail.component';
import { MemberDetailResolver } from './_resolvers/member-detail.resolver';

export const appRoutes: Routes = [
    { path: 'home' , component: HomeComponent },
    {
        path: '',
        runGuardsAndResolvers: 'always',
        canActivate: [AuthenticationGuard],
        children: [
            { path: 'members' , component: MemberListComponent, resolve: { users: MemberListResolver} },
            { path: 'members/:id' , component: MemberDetailComponent, resolve: { user: MemberDetailResolver }},
            { path: 'member/edit', component: MemberEditComponent,
                resolve: { user: MemberEditResolver },
                canDeactivate: [PreventUnsavedChanges]},
            { path: 'messages' , component: MessagesComponent, resolve: { messages: MessagesResolver } },
            { path: 'lists' , component: ListsComponent, resolve: { users: ListsResolver } },
        ]
    },
    { path: '**', redirectTo: 'home', pathMatch: 'full' }
];

