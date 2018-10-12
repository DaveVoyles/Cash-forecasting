import { Component, OnInit } from "@angular/core";
import { Router } from "@angular/router";
import { AdalService } from "adal-angular4";
import { environment } from "../../../../environments/environment";

@Component({
	selector: "app-login",
	templateUrl: "./login.component.html",
	styleUrls: ["./login.component.css"]
})
export class LoginComponent implements OnInit {
	name: string;

	constructor(private adalService: AdalService, private router: Router) {
		adalService.init(environment.config);
		console.log(this.adalService.userInfo);
	}

	ngOnInit() {
		this.adalService.handleWindowCallback();

		if (this.authenticated) {
			this.name = this.adalService.userInfo.userName;
			this.adalService
				.acquireToken("https://graph.microsoft.com")
				.subscribe(token => {});
		} else {
			this.router.navigate(["/unauthorized"]);
		}
	}

	get authenticated(): boolean {
		return this.adalService.userInfo.authenticated;
	}

	login() {
		this.adalService.login();
	}

	logout() {
		this.adalService.logOut();
	}
}
