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
    public function unavaliableRedirect($cookies) : void
    {
        if (!$this->isAdmin($cookies)) {
            header("Location: /login", true);
            die();
        }
    }
    
    public function unavaliableRequest($cookies) : void
    {
        if (!$this->isAdmin($cookies)) {
            http_response_code(403);
            die();
        }
    }

    public function validateUser(string $username, string $password) : int
    {
        $userFromDB = $this->getItemsFromDB([
            'username' => [ $username ],
            'password' => [ password_hash($password, PASSWORD_DEFAULT) ]
        ]);
        if(count($userFromDB) != 1){
            header("Location: /login", true);
            die();
        } 

        $user = $userFromDB[0];

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
