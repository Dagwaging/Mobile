//
//  RHLoginViewController.m
//  Rose-Hulman Mobile
//
//  Copyright 2012 Rose-Hulman Institute of Technology
// 
//  Licensed under the Apache License, Version 2.0 (the "License");
//  you may not use this file except in compliance with the License.
//  You may obtain a copy of the License at
//
//      http://www.apache.org/licenses/LICENSE-2.0
//
//  Unless required by applicable law or agreed to in writing, software
//  distributed under the License is distributed on an "AS IS" BASIS,
//  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//  See the License for the specific language governing permissions and
//  limitations under the License.
//

#import "RHLoginViewController.h"
#import "RHAuthenticationLoader.h"


#define kSegueIdentifier @"LoginToDirectorySearchSegue"


@interface RHLoginViewController () {
@private
    BOOL _loggingIn;
    BOOL _loggedIn;
}

@property (nonatomic, strong) IBOutlet UITextField *usernameField;

@property (nonatomic, strong) IBOutlet UITextField *passwordField;

@property (nonatomic, strong) IBOutlet UILabel *actionButtonText;

- (void)login;

- (void)logout;

@end


@implementation RHLoginViewController

@synthesize usernameField = _usernameField;
@synthesize passwordField = _passwordField;
@synthesize actionButtonText = _actionButtonText;

- (id)initWithStyle:(UITableViewStyle)style
{
    self = [super initWithStyle:style];
    if (self) {
        // Custom initialization
    }
    return self;
}

- (void)viewDidLoad
{
    [super viewDidLoad];
    
    // Uncomment the following line to preserve selection between presentations.
    // self.clearsSelectionOnViewWillAppear = NO;
    
    // Uncomment the following line to display an Edit button in the navigation bar for this view controller.
    // self.navigationItem.rightBarButtonItem = self.editButtonItem;
    
    _loggingIn = NO;
    _loggedIn = NO;
    
    if ([RHAuthenticationLoader.instance hasCredentials]) {
        [RHAuthenticationLoader.instance attemptReauthenticationWithSuccessBlock:^{
            
            self.navigationItem.title = @"Kerberos Login";
            self.navigationItem.rightBarButtonItem = nil;
            
            self.usernameField.text = [RHAuthenticationLoader.instance username];
            self.passwordField.text = @"aaaaaaaaaaaaaaa";
            
            self.actionButtonText.text = @"Log Out";
            
            _loggingIn = NO;
            _loggedIn = YES;
            
            [self performSegueWithIdentifier:kSegueIdentifier sender:nil];
            
        } failureBlock:^(NSError *error) {
            NSLog(@"Error: %@", error);
        }];
    }
}

- (BOOL)shouldAutorotateToInterfaceOrientation:(UIInterfaceOrientation)interfaceOrientation
{
    return (interfaceOrientation == UIInterfaceOrientationPortrait);
}

#pragma mark - Table View Delegate Methods

- (void)tableView:(UITableView *)tableView didSelectRowAtIndexPath:(NSIndexPath *)indexPath
{
    [tableView deselectRowAtIndexPath:indexPath animated:YES];
    
    if (indexPath.section == 2) {
        [self login];
    }
}

#pragma mark - Private Methods

- (void)login
{
    if (_loggingIn) {
        return;
    }
    
    if (_loggedIn) {
        [self logout];
        return;
    }
    
    _loggingIn = YES;
    
    [self.usernameField resignFirstResponder];
    [self.passwordField resignFirstResponder];
    
    self.navigationItem.title = @"Logging in";
    
    UIActivityIndicatorView *activityIndicator = [[UIActivityIndicatorView alloc] initWithActivityIndicatorStyle:UIActivityIndicatorViewStyleGray];
    self.navigationItem.rightBarButtonItem = [[UIBarButtonItem alloc] initWithCustomView:activityIndicator];
    [activityIndicator startAnimating];
    
    [RHAuthenticationLoader.instance authenticateWithUsername:self.usernameField.text password:self.passwordField.text successBlock:^{
        
        self.navigationItem.title = @"Kerberos Login";
        self.navigationItem.rightBarButtonItem = nil;
        
        self.passwordField.text = @"aaaaaaaaaaaaaaa";
        
        self.actionButtonText.text = @"Log Out";
        
        _loggingIn = NO;
        _loggedIn = YES;
        
        [self performSegueWithIdentifier:kSegueIdentifier sender:nil];
        
    } failureBlock:^(NSError *error) {
        [[[UIAlertView alloc] initWithTitle:@"Login Failed" message:@"Username or password incorrect" delegate:nil cancelButtonTitle:@"OK" otherButtonTitles:nil] show];
        
        self.navigationItem.title = @"Kerberos Login";
        self.navigationItem.rightBarButtonItem = nil;
        
        self.usernameField.text = @"";
        self.passwordField.text = @"";
        
        _loggedIn = NO;
        _loggingIn = NO;
        
        NSLog(@"Failed: %@", error);
    }];
}

- (void)logout {
    self.actionButtonText.text = @"Log In";
    self.usernameField.text = @"";
    self.passwordField.text = @"";
    
    [RHAuthenticationLoader.instance clearCredentials];
    
    _loggedIn = NO;
}

@end
