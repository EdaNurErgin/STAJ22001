import 'dart:ui';

import 'package:flutter/material.dart';

class BackgroundWrapper extends StatelessWidget {
  final Widget child;

  const BackgroundWrapper({super.key, required this.child});

  @override
  Widget build(BuildContext context) {
    return Stack(
      children: [
        Opacity(
          opacity: 0.5, // 🔹 Saydamlık oranı: 0.0 (tam şeffaf) - 1.0 (tam opak)
          child: Container(
            decoration: const BoxDecoration(
              image: DecorationImage(
                image: AssetImage('assets/images/logo.webp'),
                fit: BoxFit.cover,
              ),
            ),
          ),
        ),
        BackdropFilter(
          filter: ImageFilter.blur(sigmaX: 2.5, sigmaY: 2.5),
          child: Container(
            color: Colors.black.withOpacity(
              0.1,
            ), // 🔹 Blur sonrası hafif koyuluk
          ),
        ),
        child, // İçerik (ör. Scaffold)
      ],
    );
  }
}
