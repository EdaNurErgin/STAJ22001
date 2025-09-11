
import 'dart:ui';

import 'package:flutter/material.dart';

class AppWrapper extends StatelessWidget {
  final Widget child;
  final String title;
  final bool showBackButton;
  final VoidCallback? onBack; // ✅ Yeni eklendi
  final List<Widget>? actions;
  final Widget? bottomNavigationBar;

  const AppWrapper({
    super.key,
    required this.child,
    required this.title,
    this.showBackButton = true,
    this.onBack,
    this.actions,
    this.bottomNavigationBar, // ✅ constructor'a da ekledik
  });

  @override
  Widget build(BuildContext context) {
    return Stack(
      fit: StackFit.expand,
      children: [
        Container(
          decoration: const BoxDecoration(
            image: DecorationImage(
              image: AssetImage('assets/images/logo.webp'),
              fit: BoxFit.cover,
            ),
          ),
        ),
        BackdropFilter(
          filter: ImageFilter.blur(sigmaX: 2.5, sigmaY: 2.5),
          child: Container(color: Colors.black.withOpacity(0.1)),
        ),
        Scaffold(
          backgroundColor: Colors.transparent,
          appBar: AppBar(
            backgroundColor: Colors.white,
            elevation: 0,
            title: Text(title),
            leading: showBackButton
                ? IconButton(
                    icon: const Icon(Icons.arrow_back),
                    onPressed: onBack ?? () => Navigator.pop(context),
                  )
                : Padding(
                    padding: const EdgeInsets.all(8.0),
                    child: Image.asset(
                      'assets/images/logo4.png',
                      fit: BoxFit.contain,
                      height: 50,
                      width: 50,
                    ),
                  ),
            actions: actions,
          ),
          body: child,
          bottomNavigationBar: bottomNavigationBar, // ✅ Scaffold'a ekledik
        ),
      ],
    );
  }
}
