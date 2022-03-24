<?php

namespace framework\entities\users;

use framework\database\StringHelper;
use framework\entities\default_entities\DefaultEntitiesService;
use framework\entities\tokens\Token;
use framework\entities\tokens\TokensService;

class UsersService extends DefaultEntitiesService
{
    private TokensService $_tokensService;

    public function __construct()
    {
        $this->className = self::ENTITIES_NAMESPACE . "users\\User";
        $this->tableName = "pa_users";
        $this->_tokensService = new TokensService();
    }
    public function isAdmin(array $cookies) : bool
    {
        if (!empty($cookies)) {
            if (isset($cookies['userid'])) {
                $user = $this->getItemFromDB($cookies['userid']);
                if ($user->role == 9) {
                    return true;
                }
            }
        }
        return false;
    }
    public function isLoggedInUser(array $cookies) : bool
    {
        if (!empty($cookies)) {
            if (isset($cookies['userid'])) {
                $user = $this->getItemFromDB($cookies['userid']);
                if (in_array($user->role, [1,2,3,4,5,6,7,8,9])) {
                    return true;
                }
            }
        }
        return false;
    }
    public function unavaliableRedirect(array $cookies) : void
    {
        if (!$this->isAdmin($cookies)) {
            header("Location: /login", true);
            die();
        }
    }
    
    public function unavaliableRequest(array $cookies) : void
    {
        if (!$this->isAdmin($cookies)) {
            http_response_code(403);
            die();
        }
    }

    public function isAbleToRegister(string $username, string $email) : bool
    {
        $usersFromDB = $this->getItemsFromDB([
            'username' => [ $username ]
        ]);
        if(!empty($usersFromDB)){
            return false;
        }
        $usersFromDB = $this->getItemsFromDB([
            'username' => [ $username ]
        ]);
        if(!empty($usersFromDB)){
            return false;
        }
        return true;
    }

    public function registerUser(string $username, string $email, string $password) : bool
    {
        if(!$this->isAbleToRegister($username, $email)){
            return false;
        }

        $user = new User(null, $username, $email, password_hash($password, PASSWORD_DEFAULT), null);

        $this->insertItemToDB($user);

        return true;
    }

    public function validateUser(string $username, string $password) : int
    {
        $usersFromDB = $this->getItemsFromDB([
            'username' => [ $username ],
            'password' => [ password_hash($password, PASSWORD_DEFAULT) ]
        ]);
        echo password_hash($password, PASSWORD_DEFAULT);
        if(count($usersFromDB) != 1){
            header("Location: /login", true);
            die();
        } 

        $user = $usersFromDB[0];

        if(!password_verify($password, $user->password)){
            http_response_code(403);
            die();
        }

        $token = $this->_tokensService->createToken($user->id);

        setcookie("userid", $token->userid, time() + 2592000);
        setcookie("token", $token->token, $token->expires);
        setcookie("token_expires", $token->expires, $token->expires);

        return $user->role;
    }

}
