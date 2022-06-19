<?php

namespace framework\entities\users;

use framework\entities\default_entities\DefaultEntitiesService;
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
    public function hasPermission(array $cookies, int $role = 9): bool
    {
        if (!empty($cookies)) {
            if (isset($cookies['userid'])) {
                $user = $this->getItemFromDB($cookies['userid']);
                if ($user->role >= $role) {
                    return true;
                }
            }
        }
        return false;
    }
    public function isLoggedInUser(array $cookies): bool
    {
        if (!empty($cookies)) {
            if (isset($cookies['userid'])) {
                $user = $this->getItemFromDB($cookies['userid']);
                if (in_array($user->role, [1, 2, 3, 4, 5, 6, 7, 8, 9])) {
                    return true;
                }
            }
        }
        return false;
    }
    public function unavaliableRedirect(array $cookies, int $role = 9): void
    {
        if (!$this->hasPermission($cookies, $role)) {
            http_response_code(403);
            die();
        }
    }

    public function unavaliableRequest(array $cookies, int $role = 9): void
    {
        if (!$this->hasPermission($cookies, $role)) {
            http_response_code(403);
            die();
        }
    }

    public function validateUser(string $username, string $password): int
    {
        $usersFromDB = $this->getItemsFromDB([
            'username' => [$username],
        ]);
        if (count($usersFromDB) != 1) {
            http_response_code(403);
            die();
        }

        $user = $usersFromDB[0];

        if (!password_verify($password, $user->password)) {
            http_response_code(403);
            die();
        }

        if ($user->role < 1) {
            header("Location: /register/confirm_email", true);
            die();
        }

        $token = $this->_tokensService->createToken($user->id);

        unset($_COOKIE["userid"]);
        unset($_COOKIE["token"]);
        unset($_COOKIE["token_expires"]);
        setcookie("userid", $token->userid, time() + 2592000, '/');
        setcookie("token", $token->token, $token->expires, '/');
        setcookie("token_expires", $token->expires, $token->expires, '/');

        return $user->role;
    }

    public function getUserInfoByCookie(array $cookies): User
    {
        $user = $this->getItemFromDB($cookies['userid']);
        $user->password = "";

        return $user;
    }
}
